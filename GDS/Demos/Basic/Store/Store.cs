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
using static GDS.Basic.Factory;
using GDS.Basic.Events;

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
            Bus.Subscribe<SellItemEvent>(e => OnSellItem(e as SellItemEvent));
            Bus.Subscribe<CollectAllEvent>(e => OnCollectAll(e as CollectAllEvent));
            Bus.Subscribe<HotbarUseEvent>(e => OnHotbarUse(e as HotbarUseEvent));
            Bus.Subscribe<ToggleInventoryEvent>(e => OnToggleInventory(e as ToggleInventoryEvent));
            Bus.Subscribe<CloseInventoryEvent>(e => OnCloseInventory(e as CloseInventoryEvent));
            Bus.Subscribe<OpenSideWindowEvent>(e => OnOpenSideWindow(e as OpenSideWindowEvent));
            Bus.Subscribe<CloseWindowEvent>(e => OnCloseWindow(e as CloseWindowEvent));
            Bus.Subscribe<DropDraggedItemEvent>(e => OnDropDraggedItem(e as DropDraggedItemEvent));
            Bus.Subscribe<DestroyDraggedItemEvent>(e => OnDestroyDraggedItem(e as DestroyDraggedItemEvent));

            // Initilize by calling Reset
            Reset();
        }


        public static readonly EventBus Bus = new();
        public static readonly Store Instance = new();

        public readonly Main Main = CreateListBag<Main>("main", 40);
        public readonly SetBag Equipment = CreateSetBag("equipment", Factory.CreateEquipmentSlots());
        public readonly ListBag Hotbar = CreateListBag("hotbar", 5).AcceptsFn(Basic.Filters.Consumable);
        public readonly ListBag Chest = CreateListBag("chest", 10).AcceptsFn(Core.Filters.Nothing);
        public readonly Vendor Vendor = CreateListBag<Vendor>("vendor", 20);
        public readonly Stash Stash = CreateListBag<Stash>("stash", 80);
        public readonly CraftingBench CraftingBench = CreateCraftingBench("crafting-bench", 3).AcceptsFn(Basic.Filters.Material);
        // public readonly ListBag Ground = CreateListBag("ground", 20).AcceptsFn(Core.Filters.Nothing);

        public readonly Observable<Item> DraggedItem = new(Item.NoItem);
        public readonly Observable<bool> IsInventoryOpen = new(false);
        public readonly Observable<Bag> SideWindow = new(Bag.NoBag);

        Bag lastItemParent;

        /// <summary>
        /// Resets the Store state
        /// Adds initial items to all inventories
        /// </summary>
        void Reset() {

            Debug.Log($"Reseting ".Yellow() + "[Basic Store]".Gray());

            IsInventoryOpen.SetValue(false);
            SideWindow.SetValue(Bag.NoBag);

            Main.SetState(
                CreateItem(Bases.Apple, Rarity.NoRarity, 10),
                CreateItem(Bases.Wood, Rarity.NoRarity, 100),
                CreateItem(Bases.Wood, Rarity.NoRarity, 200),
                CreateItem(Bases.Steel, Rarity.NoRarity, 100),
                CreateItem(Bases.Gem, Rarity.NoRarity, 50),
                CreateItem(Bases.SteelBoots, Rarity.Magic, 50),
                CreateItem(Bases.ShortSword, Rarity.Rare),
                CreateItem(Bases.Axe, Rarity.Common),
                CreateItem(Bases.LeatherArmor, Rarity.Unique),
                CreateItem(Bases.LeatherGloves, Rarity.Magic)
            );

            Equipment.SetState(
                (SlotType.Helmet.ToString(), CreateItem(Bases.WarriorHelmet, Rarity.Rare)),
                (SlotType.Boots.ToString(), CreateItem(Bases.SteelBoots, Rarity.Magic))
            );


            Vendor.SetState(
                CreateItem(Bases.Apple, Rarity.NoRarity, 20),
                CreateItem(Bases.Mushroom, Rarity.NoRarity, 20),
                CreateItem(Bases.Potion, Rarity.NoRarity, 20),
                CreateItem(Bases.Wood, Rarity.NoRarity, 200),
                CreateItem(Bases.Steel, Rarity.NoRarity, 100),
                CreateItem(Bases.Gem, Rarity.NoRarity, 50),

                CreateItem(Bases.WarriorHelmet, Rarity.Rare),
                CreateItem(Bases.LeatherGloves, Rarity.Unique),
                CreateItem(Bases.LeatherArmor, Rarity.Unique),
                CreateItem(Bases.SteelBoots, Rarity.Magic),

                CreateItem(Bases.ShortSword, Rarity.Rare),
                CreateItem(Bases.LongSword, Rarity.Common),
                CreateItem(Bases.Axe, Rarity.Common)
            );

            Chest.SetState(
                CreateItem(Bases.ShortSword, Rarity.Common),
                CreateItem(Bases.LongSword, Rarity.Magic),
                CreateItem(Bases.Axe, Rarity.Rare),
                CreateItem(Bases.GoldRing, Rarity.Unique),
                CreateItem(Bases.Wood, Rarity.NoRarity, 100),
                CreateItem(Bases.Steel, Rarity.NoRarity, 100),
                CreateItem(Bases.Gem, Rarity.NoRarity, 20),
                CreateItem(Bases.Mushroom, Rarity.NoRarity, 20)
            );

            // Ground.SetState();
            Stash.SetState();
            CraftingBench.SetState();
            Hotbar.SetState();
        }

        void OnAddItem(AddItemEvent e) {
            LogEvent(e);
            Main.AddItems(e.Item);
            EventBus.GlobalBus.Publish(CreateMessageEvent("Picked", e.Item));
        }

        /// <summary>
        /// PickItem event handler
        /// If picking is successful, sets the new state of Dragged Item and notifies the 
        /// inventory from which the item was picked
        /// Handles various cases like ctrl+click and shift+click
        /// </summary>
        /// <param name="e"></param>
        void OnPickItem(PickItemEvent e) {
            // TODO: find if this check can happen in a prior step
            if (e.Slot.IsEmpty()) return;
            LogEvent(e);
            var ctrl = e.EventModifiers.HasFlag(EventModifiers.Control);
            var shift = e.EventModifiers.HasFlag(EventModifiers.Shift);
            var from = e.Bag;
            var to = from is Main ? SideWindow.Value : Main;
            var move = ctrl && to is not NoBag;
            var unstack = shift && e.Item.ItemBase.Stackable;
            lastItemParent = e.Bag;

            var (success, @event) = (move, unstack) switch {
                (true, true) => Extensions.UnstackAndMoveItem(from, to, e.Item, e.Slot),
                (true, _) => Extensions.MoveItem(from, to, e.Item, e.Slot),
                (_, true) => Extensions.UnstackItem(from, e.Item, e.Slot, DraggedItem),
                _ => Extensions.PickItem(from, e.Item, e.Slot, DraggedItem)
            };

            if (@event is not NoEvent) EventBus.GlobalBus.Publish(@event);

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
            bool didPlace;

            switch (lastItemParent, e.Bag) {
                case (Basic.Vendor, Basic.Vendor): // Should swap the item
                    DraggedItem.SetValue(e.Slot.Item);
                    break;

                case (Vendor from, Bag to) when to is not Basic.Vendor: // Should trigger the Buy flow
                    (didPlace, _) = e.Bag.PlaceItem(e.Item.Clone(), e.Slot, DraggedItem);
                    if (!didPlace) return;
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Bought", e.Item));
                    break;

                case (Bag from, Vendor to) when from is not Basic.Vendor: // Should trigger the Sell flow
                    DraggedItem.SetValue(Item.NoItem);
                    EventBus.GlobalBus.Publish(CreateMessageEvent("Sold", e.Item));
                    break;

                default: // Should place the item (default action)
                    e.Bag.PlaceItem(e.Item, e.Slot, DraggedItem);
                    break;
            }
        }

        void OnBuyItem(BuyItemEvent e) {
            LogEvent(e);
            var pos = Main.AddItems(e.Item.Clone());
            if (pos == null) return;
            EventBus.GlobalBus.Publish(CreateMessageEvent("Bought", e.Item));
        }

        void OnSellItem(SellItemEvent e) {
            LogEvent(e);
            DraggedItem.SetValue(Item.NoItem);
            EventBus.GlobalBus.Publish(CreateMessageEvent("Sold", e.Item));
        }

        /// <summary>
        /// Trigerred when an item is dropped "on the ground".
        /// </summary>
        /// <param name="e"></param>
        void OnDropDraggedItem(DropDraggedItemEvent e) {
            LogEvent(e);
            EventBus.GlobalBus.Publish(CreateMessageEvent("Dropped", DraggedItem.Value));
            EventBus.GlobalBus.Publish(new DropItemSuccessEvent(DraggedItem.Value));
            DraggedItem.SetValue(Item.NoItem);
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
            var didNotFit = Main.AddItems(items.ToArray());
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
            EventBus.GlobalBus.Publish(CreateMessageEvent("Consumed", item, false));
            EventBus.GlobalBus.Publish(new ConsumeItemSuccessEvent(item));
        }

        void OnToggleInventory(ToggleInventoryEvent e) {
            LogEvent(e);
            IsInventoryOpen.SetValue(!IsInventoryOpen.Value);
            if (IsInventoryOpen.Value == true) return;
            SideWindow.SetValue(Bag.NoBag);

            // TODO: implement return item to initial slot instead of dropping
            if (DraggedItem.Value is NoItem) return;
            OnDropDraggedItem(new DropDraggedItemEvent());

        }

        void OnCloseInventory(CloseInventoryEvent e) {
            LogEvent(e);
            IsInventoryOpen.SetValue(false);
            SideWindow.SetValue(Bag.NoBag);

            // TODO: implement return item to initial slot instead of dropping
            if (DraggedItem.Value is NoItem) return;
            OnDropDraggedItem(new DropDraggedItemEvent());
        }

        void OnOpenSideWindow(OpenSideWindowEvent e) {
            LogEvent(e);

            SideWindow.SetValue(e.Bag);
            IsInventoryOpen.SetValue(true);
        }

        void OnCloseWindow(CloseWindowEvent e) {
            LogEvent(e);
            if (e.Bag == Main) IsInventoryOpen.SetValue(false);
            SideWindow.SetValue(Bag.NoBag);
        }


    }
}