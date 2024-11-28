using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using GDS.Core;
using GDS.Core.Events;
using static GDS.Core.LogUtil;
using static GDS.Core.InventoryFactory;
using static GDS.Core.InventoryExtensions;
using static GDS.Basic.BasicItemFactory;
using static GDS.Basic.BasicInventoryFactory;

namespace GDS.Basic {

    /// <summary>
    /// `Store` is a singleton that contains all the system state, listens to UI events and updates the state
    /// Only the Store can modify it's internal state
    /// </summary>
    public class Store {
        public Store() {
            Debug.Log($"Initializing ".Yellow() + "[Basic Store]".Gray());
            // Listen (subscribe) to reset event - it is triggered when entering play mode
            EventBus.GlobalBus.Subscribe<ResetEvent>(e => Reset());

            Bus.Subscribe<ResetEvent>(e => Reset());
            Bus.Subscribe<AddItemEvent>(e => OnAddItem(e as AddItemEvent));
            Bus.Subscribe<PickItemEvent>(e => OnPickItem(e as PickItemEvent));
            Bus.Subscribe<PlaceItemEvent>(e => OnPlaceItem(e as PlaceItemEvent));
            Bus.Subscribe<BuyItemEvent>(e => OnBuyItem(e as BuyItemEvent));
            Bus.Subscribe<CollectAllEvent>(e => OnCollectAll(e as CollectAllEvent));
            Bus.Subscribe<HotbarUseEvent>(e => OnHotbarUse(e as HotbarUseEvent));
            Bus.Subscribe<ToggleInventoryEvent>(e => OnToggleInventory(e as ToggleInventoryEvent));
            Bus.Subscribe<CloseInventoryEvent>(e => OnCloseInventory(e as CloseInventoryEvent));
            Bus.Subscribe<ToggleWindowEvent>(e => OnToggleWindow(e as ToggleWindowEvent));
            Bus.Subscribe<DropDraggedItemEvent>(e => OnDropDraggedItem(e as DropDraggedItemEvent));
            Bus.Subscribe<DestroyDraggedItemEvent>(e => OnDestroyDraggedItem(e as DestroyDraggedItemEvent));
            // Initilize by calling Reset
            Reset();
        }


        public static readonly EventBus Bus = new();
        public static readonly Store Instance = new();

        public readonly SetBag Equipment = CreateSetBag("Equipment", DB.EquipmentSlots);
        public readonly ListBag Inventory = CreateListBag("MainInventory", 40);
        public readonly ListBag Hotbar = CreateListBag("Hotbar", 5).AcceptsFn(Basic.Accepts.Consumable);
        public readonly ListBag Chest = CreateListBag("chest", 5).AcceptsFn(Core.Accepts.Nothing);
        public readonly ListBag Ground = CreateListBag("ground", 20).AcceptsFn(Core.Accepts.Nothing);
        public readonly Vendor Vendor = CreateListBag<Vendor>("vendor", 20);
        public readonly CraftingBench CraftingBench = CreateCraftingBench("crafting-bench", 3).AcceptsFn(Basic.Accepts.Material);
        public readonly Stash Stash = CreateListBag<Stash>("stash", 80);

        public readonly Observable<Item> DraggedItem = new(Item.NoItem);
        public readonly Observable<bool> IsInventoryOpen = new(true);
        public readonly Observable<string> sideWindowId = new("");

        public Bag OtherInventory = Bag.NoBag;
        public int LastStashTabIndex = 1;

        Bag lastItemParent;

        /// <summary>
        /// Resets the Store state
        /// Adds initial items to all inventories
        /// </summary>
        void Reset() {

            Debug.Log($"Reseting ".Yellow() + "[Basic Store]".Gray());

            Inventory.SetState(
                Create(BaseId.Apple, Rarity.NoRarity, 10),
                Create(BaseId.Wood, Rarity.NoRarity, 100),
                Create(BaseId.Wood, Rarity.NoRarity, 200),
                Create(BaseId.Steel, Rarity.NoRarity, 100),
                Create(BaseId.Gem, Rarity.NoRarity, 50),
                Create(BaseId.SteelBoots, Rarity.Magic, 50),
                Create(BaseId.ShortSword, Rarity.Rare),
                Create(BaseId.Axe, Rarity.Common),
                Create(BaseId.LeatherArmor, Rarity.Unique),
                Create(BaseId.LeatherGloves, Rarity.Magic)
            );

            Equipment.SetState(
                (SlotType.Helmet.ToString(), Create(BaseId.WarriorHelmet, Rarity.Rare)),
                (SlotType.Boots.ToString(), Create(BaseId.SteelBoots, Rarity.Magic))
            );


            Vendor.SetState(
                Create(BaseId.Apple, Rarity.NoRarity, 20),
                Create(BaseId.Mushroom, Rarity.NoRarity, 20),
                Create(BaseId.Potion, Rarity.NoRarity, 20),
                Create(BaseId.Wood, Rarity.NoRarity, 200),
                Create(BaseId.Steel, Rarity.NoRarity, 100),
                Create(BaseId.Gem, Rarity.NoRarity, 50),
                Create(BaseId.SteelBoots, Rarity.Magic),
                Create(BaseId.ShortSword, Rarity.Rare),
                Create(BaseId.LeatherArmor, Rarity.Unique),
                Create(BaseId.LongSword, Rarity.Common)
            );

            Chest.SetState(
                Create(BaseId.ShortSword, Rarity.Common),
                Create(BaseId.LongSword, Rarity.Magic),
                Create(BaseId.Axe, Rarity.Rare),
                Create(BaseId.GoldRing, Rarity.Unique)
            );

            Ground.SetState();
            Stash.SetState();
            Hotbar.SetState();

            CraftingBench.Clear();
            CraftingBench.Notify();
        }

        void OnAddItem(AddItemEvent e) {
            LogEvent(e);
            var pos = Inventory.AddItems(e.Item);
            if (pos == null) return;
            Inventory.Notify();
        }

        /// <summary>
        /// PickItem event handler
        /// If picking is successful, sets the new state of Dragged Item and notifies the 
        /// inventory from which the item was picked
        /// Handles various cases like ctrl+click and shift+click
        /// </summary>
        /// <param name="e"></param>
        void OnPickItem(PickItemEvent e) {
            LogEvent(e);

            var ctrl = e.EventModifiers.HasFlag(EventModifiers.Control);
            var shift = e.EventModifiers.HasFlag(EventModifiers.Shift);
            var from = e.Bag;
            var to = from == Inventory ? OtherInventory : Inventory;
            var slot = e.Slot;
            lastItemParent = e.Bag;

            switch (from, slot, ctrl, shift) {
                // ctrl+click CraftingoutcomeSlot -> craft and move flow (move, decrement materials, message)
                case (Basic.CraftingBench bag, CraftingOutcomeSlot, true, _):
                    LogTodo("should trigger craft and move flow");
                    if (Inventory.IsFull()) { EventBus.GlobalBus.Publish(new MessageEvent("Inventory full".Yellow())); return; }
                    var craftSuccess = bag.ConsumeOneOfEachMaterials();
                    if (!craftSuccess) return;
                    Inventory.AddItems(e.Item.Clone());
                    Inventory.Notify();
                    bag.Notify();
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Crafted", e.Item));
                    break;

                // click CraftingOutcomeSlot -> craft and pick flow (pick, decrement materials, message)
                case (Basic.CraftingBench bag, CraftingOutcomeSlot, _, _):
                    DraggedItem.SetValue(e.Item.Clone());
                    craftSuccess = bag.ConsumeOneOfEachMaterials();
                    if (!craftSuccess) return;
                    bag.Notify();
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Crafted", e.Item));
                    break;

                // pick Vendor item -> buy flow (message)
                case (Basic.Vendor, _, false, _):
                    DraggedItem.SetValue(e.Item);
                    break;

                // ctrl+click Vendor item -> buy and move flow (move, message)
                case (Basic.Vendor, _, true, _):
                    OnBuyItem(new BuyItemEvent(e.Item));
                    break;

                // ctrl+click when other is Vendor -> sell flow (move/remove, message)
                case (_, _, true, _) when to is Vendor:
                    DraggedItem.SetValue(Item.NoItem);
                    e.Bag.RemoveItem(e.Item);
                    e.Bag.Notify();
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Sold", e.Item));
                    break;

                // ctrl+click -> move item to other
                case (_, _, true, _):
                    var moveSuccess = InventoryExtensions.MoveItem(from, to, e.Item);
                    if (!moveSuccess) return;
                    from.Notify();
                    to.Notify();
                    break;

                // shit+click when item is Stackable -> unstack
                case (_, _, _, true) when e.Item.ItemBase.Stackable && e.Item.ItemData.Quant > 1:
                    var Quant = e.Item.ItemData.Quant;
                    var half = (int)Math.Floor(e.Item.ItemData.Quant / 2f);
                    var newDraggedItem = e.Item.Clone(e.Item.ItemData with { Quant = half });
                    var newBagItem = e.Item.Clone(e.Item.ItemData with { Quant = Quant - half });
                    DraggedItem.SetValue(newDraggedItem);
                    e.Bag.SetItem(e.Slot, newBagItem);
                    e.Bag.Notify();
                    break;

                // default (click) -> pick item
                default:
                    var (success, replacedItem) = from.PickItem(e.Item, e.Slot);
                    if (!success) return;
                    DraggedItem.SetValue(replacedItem);
                    from.Notify();
                    Bus.Publish(new PlaySoundEvent());
                    break;
            }
        }

        /// <summary>
        /// PlaceItem event handler
        /// If placing is successful, sets the new state of the Dragged Item and notifies the
        /// inventory in which the item was placed
        /// Handles various edge cases (like picking an Item from the Vendor and trying to sell it back)
        /// </summary>
        /// <param name="e"></param>
        void OnPlaceItem(PlaceItemEvent e) {
            LogEvent(e);
            bool didPlace; Item replacedItem;

            switch (lastItemParent, e.Bag) {
                case (Basic.Vendor, Basic.Vendor): // Should swap the item
                    DraggedItem.SetValue(e.Slot.Item);
                    break;

                case (Vendor from, Bag to) when to is not Basic.Vendor: // Should trigger the Buy flow
                    (didPlace, replacedItem) = e.Bag.PlaceItem(e.Item.Clone(), e.Slot);
                    if (!didPlace) return;
                    DraggedItem.SetValue(replacedItem);
                    e.Bag.Notify();
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Bought", e.Item));
                    break;

                case (Bag from, Vendor to) when from is not Basic.Vendor: // Should trigger the Sell flow
                    DraggedItem.SetValue(Item.NoItem);
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Sold", e.Item));
                    break;

                default: // Should place the item (default action)
                    (didPlace, replacedItem) = e.Bag.PlaceItem(e.Item, e.Slot);
                    if (!didPlace) return;
                    DraggedItem.SetValue(replacedItem);
                    e.Bag.Notify();
                    break;
            }
        }

        void OnBuyItem(BuyItemEvent e) {
            LogEvent(e);
            var pos = Inventory.AddItems(e.Item.Clone());
            if (pos == null) return;
            Inventory.Notify();
            EventBus.GlobalBus.Publish(CreateMessageEvent("Bought", e.Item));
        }

        /// <summary>
        /// Trigerred when an item is dropped "on the ground".
        /// The action can fail (ground items bag full). 
        /// In either case a message event is triggered
        /// </summary>
        /// <param name="e"></param>
        void OnDropDraggedItem(DropDraggedItemEvent e) {
            LogEvent(e);
            var success = Ground.AddItem(DraggedItem.Value);
            if (!success) {
                EventBus.GlobalBus.Publish(CreateMessageEvent("Could not drop", DraggedItem.Value));
                return;
            }
            EventBus.GlobalBus.Publish(CreateMessageEvent("Dropped", DraggedItem.Value));
            DraggedItem.SetValue(Item.NoItem);
            Ground.Notify();
        }

        /// <summary>
        /// Trigerred when an item is destroyed
        /// </summary>
        /// <param name="e"></param>
        void OnDestroyDraggedItem(DestroyDraggedItemEvent e) {
            LogEvent(e);
            EventBus.GlobalBus.Publish(CreateMessageEvent("Destroyed (!)".Red(), DraggedItem.Value));
            DraggedItem.SetValue(Item.NoItem);
        }

        /// <summary>
        /// Moves all items from a bag to main inventory
        /// The action can partially fail (not all items can fit), in which case, the items that 
        /// did not fit, will remain in the source bag
        /// </summary>
        /// <param name="e"></param>
        void OnCollectAll(CollectAllEvent e) {
            LogEvent(e);
            var items = e.Items.NotEmpty();
            var didNotFit = Inventory.AddItems(items.ToArray());
            Inventory.Notify();
            e.Bag.SetState(didNotFit.ToArray());

            EventBus.GlobalBus.Publish(new MessageEvent((items.Count() - didNotFit.Count()).ToString().Green() + " items collected".Gray()));

            if (didNotFit.Count() > 0) {
                EventBus.GlobalBus.Publish(new MessageEvent(didNotFit.Count().ToString().Red() + " items did not fit into inventory".Gray()));
            }
        }

        /// <summary>
        /// Consumes an item in the hot-bar.
        /// If item quantity is 0 after the action, it becomes a `NoItem`
        /// </summary>
        /// <param name="e"></param>
        void OnHotbarUse(HotbarUseEvent e) {
            LogEvent(e);
            var index = e.indexPlusOne - 1;
            var (didConsume, item) = Hotbar.Consume(index);
            if (!didConsume) return;
            Hotbar.Notify();
            EventBus.GlobalBus.Publish(CreateMessageEvent("Consumed", item, false));
        }

        void OnToggleInventory(ToggleInventoryEvent e) {
            LogEvent(e);
            IsInventoryOpen.SetValue(!IsInventoryOpen.Value);
            if (IsInventoryOpen.Value == true) return;
            sideWindowId.SetValue("");
        }

        void OnCloseInventory(CloseInventoryEvent e) {
            LogEvent(e);
            IsInventoryOpen.SetValue(false);
            sideWindowId.SetValue("");
        }

        void OnToggleWindow(ToggleWindowEvent e) {
            LogEvent(e);

            var nextWindowId = sideWindowId.Value == e.Id ? "" : e.Id;
            sideWindowId.SetValue(nextWindowId);
            IsInventoryOpen.SetValue(true);

            OtherInventory = sideWindowId.Value switch {
                string value when value == "stash" => Stash,
                string value when value == "vendor" => Vendor,
                string value when value == "crafting-bench" => CraftingBench,
                _ => Bag.NoBag
            };
        }

        MessageEvent CreateMessageEvent(string action, Item item, bool showQuant = true) {
            var quantText = (showQuant && item.ItemBase.Stackable) ? $" ({item.ItemData.Quant})" : "";
            return new MessageEvent(action + " " + $"[{item.Name()}{quantText}]".Blue());
        }


    }
}