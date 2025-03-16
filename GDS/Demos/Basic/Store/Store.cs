using System.Linq;
using UnityEngine;
using GDS.Core;
using GDS.Core.Events;
using GDS.Basic.Events;
using static GDS.Core.LogUtil;
using static GDS.Core.InventoryFactory;
using static GDS.Core.InventoryExtensions;
using static GDS.Basic.Factory;

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
            Bus.Subscribe<BuyRandomItem>(e => OnBuyRandomItem(e as BuyRandomItem));
            Bus.Subscribe<RerollShop>(e => OnRerollShop(e as RerollShop));
            Bus.Subscribe<SellItemEvent>(e => OnSellItem(e as SellItemEvent));
            Bus.Subscribe<CollectAllEvent>(e => OnCollectAll(e as CollectAllEvent));
            Bus.Subscribe<HotbarUseEvent>(e => OnHotbarUse(e as HotbarUseEvent));
            Bus.Subscribe<ToggleInventoryEvent>(e => OnToggleInventory(e as ToggleInventoryEvent));
            Bus.Subscribe<ToggleCharacterSheet>(e => OnToggleCharacterSheet(e as ToggleCharacterSheet));
            Bus.Subscribe<CloseInventoryEvent>(e => OnCloseInventory(e as CloseInventoryEvent));
            Bus.Subscribe<OpenSideWindowEvent>(e => OnOpenSideWindow(e as OpenSideWindowEvent));
            Bus.Subscribe<OpenSideWindowByIdEvent>(e => OnOpenSideWindowById(e as OpenSideWindowByIdEvent));
            Bus.Subscribe<CloseWindowEvent>(e => OnCloseWindow(e as CloseWindowEvent));
            Bus.Subscribe<DropDraggedItemEvent>(e => OnDropDraggedItem(e as DropDraggedItemEvent));
            Bus.Subscribe<DestroyDraggedItemEvent>(e => OnDestroyDraggedItem(e as DestroyDraggedItemEvent));

            CharacterSheet = new CharacterSheet(Equipment);
            SideWindow.OnChange += value => SideWindowBag.SetValue(value is Bag b ? b : Bag.NoBag);

            // Initilize by calling Reset
            Reset();
        }

        public static readonly EventBus Bus = new();
        public static readonly Store Instance = new();
        public static Observable<ColorSpace> ColorSpace = new(UnityEngine.ColorSpace.Uninitialized);

        public readonly Main Main = CreateListBag<Main>("main", 40);
        public readonly SetBag Equipment = CreateSetBag("equipment", Factory.CreateEquipmentSlots());
        public readonly ListBag Hotbar = CreateListBag("hotbar", 5) with { Accepts = Basic.Filters.Consumable };
        public readonly Chest Chest = CreateListBag<Chest>("chest", 10) with { Accepts = Core.Filters.Nothing };
        public readonly Vendor MaterialsShop = CreateListBag<Vendor>("materials-shop", 20) with { Infinite = true };
        public readonly Vendor EquipmentShop = CreateListBag<Vendor>("equipment-shop", 20);
        public readonly CraftingBench CraftingBench = CreateCraftingBench("crafting-bench", 3) with { Accepts = Basic.Filters.Material };
        public readonly Stash Stash = CreateListBag<Stash>("stash", 80);

        public readonly CharacterSheet CharacterSheet;

        public readonly Observable<Item> DraggedItem = new(Item.NoItem);
        public readonly Observable<bool> IsInventoryOpen = new(false);
        public readonly Observable<object> SideWindow = new(Bag.NoBag);
        public readonly Observable<Bag> SideWindowBag = new(Bag.NoBag);
        public readonly Observable<int> Gold = new(10000);

        Bag lastItemParent;

        /// <summary>
        /// Resets the Store state
        /// Adds initial items to all inventories
        /// </summary>
        void Reset() {

            Debug.Log($"Reseting ".Yellow() + "[Basic Store]".Gray());

            IsInventoryOpen.SetValue(false);
            SideWindow.SetValue(Bag.NoBag);
            Gold.SetValue(10000);

            Main.SetState(
                CreateItem(Bases.Apple, Rarity.NoRarity, 10),
                CreateItem(Bases.Wood, Rarity.NoRarity, 100),
                CreateItem(Bases.Wood, Rarity.NoRarity, 200),
                CreateItem(Bases.Steel, Rarity.NoRarity, 100),
                CreateItem(Bases.Gem, Rarity.NoRarity, 50),
                CreateItem(Bases.SteelBoots, Rarity.Magic),
                CreateItem(Bases.ShortSword, Rarity.Rare),
                CreateItem(Bases.Axe, Rarity.Common),
                CreateItem(Bases.LeatherArmor, Rarity.Unique),
                CreateItem(Bases.LeatherGloves, Rarity.Magic)
            );

            Equipment.SetState(
                (SlotType.Helmet.ToString(), CreateItem(Bases.WarriorHelmet, Rarity.Rare)),
                (SlotType.Boots.ToString(), CreateItem(Bases.SteelBoots, Rarity.Magic))
            );

            MaterialsShop.SetState(
                CreateItem(Bases.Apple, Rarity.NoRarity, 20),
                CreateItem(Bases.Mushroom, Rarity.NoRarity, 20),
                CreateItem(Bases.Potion, Rarity.NoRarity, 20),
                CreateItem(Bases.Wood, Rarity.NoRarity, 200),
                CreateItem(Bases.Steel, Rarity.NoRarity, 100),
                CreateItem(Bases.Gem, Rarity.NoRarity, 50)
            );

            EquipmentShop.SetState(
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

            Stash.SetState();
            CraftingBench.SetState();
            Hotbar.SetState();
        }

        void OnAddItem(AddItemEvent e) {
            LogEvent(e);
            Main.AddItems(e.Item);
            Bus.Publish(new CollectItemSuccess(e.Item));
        }

        /// <summary>
        /// PickItem event handler
        /// If picking is successful, sets the new state of Dragged Item and notifies the 
        /// inventory from which the item was picked
        /// Handles various cases like ctrl+click and shift+click
        /// </summary>        
        void OnPickItem(PickItemEvent e) {
            // TODO: find if this check can happen in a prior step
            if (e.Slot.IsEmpty()) return;
            LogEvent(e);
            lastItemParent = e.Bag;
            var ctrl = e.EventModifiers.HasFlag(EventModifiers.Control);
            var shift = e.EventModifiers.HasFlag(EventModifiers.Shift);
            var from = e.Bag;
            var to = SideWindowBag.Value == from ? Main : SideWindowBag.Value;
            var move = ctrl && to is not NoBag;
            var unstack = shift && e.Item.ItemBase.Stackable && e.Item.Quant() > 1;
            if (e.Bag is Vendor b && b.Infinite) unstack = false;
            if (e.Bag is Vendor && e.Item.Cost() > Gold.Value) {
                Bus.Publish(new ActionFail("Not enough gold", Severity.Warning));
                return;
            }

            var ev = (move, unstack) switch {
                (true, _) => Extensions.MoveItem(from, to, e.Item, e.Slot),
                (_, true) => Extensions.UnstackItem(from, e.Item, e.Slot, DraggedItem),
                _ => Extensions.PickItem(from, e.Item, e.Slot, DraggedItem)
            };

            Bus.Publish(ev);

            if (ev is SellItemSuccess ev0) Gold.SetValue(Gold.Value + ev0.Item.SellValue());
            if (ev is BuyItemSuccess ev1) Gold.SetValue(Gold.Value - ev1.Item.Cost());
        }

        /// <summary>
        /// PlaceItem event handler
        /// If placing is successful, sets the new state of the Dragged Item and notifies the
        /// inventory in which the item was placed
        /// Handles various edge cases (like picking an Item from the Vendor and trying to sell it back)
        /// </summary>
        void OnPlaceItem(PlaceItemEvent e) {
            LogEvent(e);
            var shift = e.EventModifiers.HasFlag(EventModifiers.Shift);
            var stack = shift && DraggedItem.Value.CanUnstackTo(e.Bag, e.Slot);

            var (replaced, ev) = (lastItemParent, e.Bag, stack) switch {
                (_, _, true) => Extensions.UnstackDraggedItem(e.Bag, e.Slot, DraggedItem),
                _ => Extensions.PlaceItem(e.Bag, e.Item, e.Slot, DraggedItem)
            };

            Bus.Publish(ev);

            if (replaced is not NoItem) Bus.Publish(new PickItemSuccess(replaced));
        }

        void OnBuyItem(BuyItemEvent e) {
            LogEvent(e);
            if (Main.AddItem(e.Item)) Bus.Publish(new BuyItemSuccess(e.Item));
            else Bus.Publish(CustomEvent.Fail);
        }

        void OnBuyRandomItem(BuyRandomItem e) {
            LogEvent(e);
            if (e.Bag is not Vendor bag) return;
            if (e.Bag.IsEmpty()) return;
            var index = UnityEngine.Random.Range(0, bag.NonEmptySlots().ToList().Count);
            var item = bag.Slots[index].Item;
            var success = bag.Infinite ? Main.AddItem(item.Clone()) : bag.MoveItem(Main, item);
            if (success) Bus.Publish(new BuyItemSuccess(item));
            else Bus.Publish(CustomEvent.Fail);

            if (success) Gold.SetValue(Gold.Value - item.Cost());
        }

        void OnRerollShop(RerollShop e) {
            LogEvent(e);
            if (e.Bag is not Vendor bag) return;
            bag.SetState(
                CreateItem(Bases.WarriorHelmet, Rarity.Rare),
                CreateItem(Bases.LeatherGloves, Rarity.Unique),
                CreateItem(Bases.LeatherArmor, Rarity.Unique),
                CreateItem(Bases.SteelBoots, Rarity.Magic),
                CreateItem(Bases.ShortSword, Rarity.Rare),
                CreateItem(Bases.LongSword, Rarity.Common),
                CreateItem(Bases.Axe, Rarity.Common)
            );

        }

        void OnSellItem(SellItemEvent e) {
            LogEvent(e);
            DraggedItem.SetValue(Item.NoItem);
            Bus.Publish(new SellItemSuccess(e.Item));
            Gold.SetValue(Gold.Value + e.Item.SellValue());
        }

        /// <summary>
        /// Trigerred when an item is dropped "on the ground".
        /// </summary>
        void OnDropDraggedItem(DropDraggedItemEvent e) {
            LogEvent(e);
            Bus.Publish(new DropItemSuccess(DraggedItem.Value));
            DraggedItem.SetValue(Item.NoItem);
        }

        /// <summary>
        /// Trigerred when an item is destroyed
        /// </summary>
        void OnDestroyDraggedItem(DestroyDraggedItemEvent e) {
            LogEvent(e);
            Bus.Publish(new DestroyItemSuccess(DraggedItem.Value));
            DraggedItem.SetValue(Item.NoItem);
        }

        /// <summary>
        /// Moves all items from a bag to main inventory
        /// The action can partially fail (not all items can fit), in which case, the items that 
        /// did not fit, will remain in the source bag
        /// </summary>
        void OnCollectAll(CollectAllEvent e) {
            LogEvent(e);
            var items = e.Items.NotEmpty();
            if (items.Count() == 0) return;
            var didNotFit = Main.AddItems(items.ToArray());
            e.Bag.SetState(didNotFit.ToArray());

            Bus.Publish(new PickItemSuccess(items.ToArray()[0]));
            if (didNotFit.Count() > 0) {
                Bus.Publish(new ActionFail("Could not fit " + didNotFit.Count().ToString().Red() + " items"));
            }
        }

        /// <summary>
        /// Consumes an item in the hot-bar.
        /// If item quantity is 0 after the action, it becomes a `NoItem`
        /// </summary>
        void OnHotbarUse(HotbarUseEvent e) {
            LogEvent(e);
            var index = e.indexPlusOne - 1;
            var (didConsume, item) = Hotbar.Consume(index);
            if (!didConsume) return;
            Bus.Publish(new ConsumeItemSuccess(item));
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

        void OnToggleCharacterSheet(ToggleCharacterSheet e) {
            LogEvent(e);
            if (SideWindow.Value is CharacterSheet) SideWindow.SetValue(Bag.NoBag);
            else SideWindow.SetValue(CharacterSheet);
            IsInventoryOpen.SetValue(true);

        }

        void OnOpenSideWindowById(OpenSideWindowByIdEvent e) {
            LogEvent(e);

            Bag bag = e.Id switch {
                "materials-shop" => MaterialsShop,
                "equipment-shop" => EquipmentShop,
                "crafting-bench" => CraftingBench,
                "chest" => Chest,
                "stash" => Stash,
                _ => Bag.NoBag
            };

            if (bag == Bag.NoBag) {
                LogWarning("Unknown bag id: " + e.Id.Red());
                return;
            }

            SideWindow.SetValue(bag);
            IsInventoryOpen.SetValue(true);
        }
    }
}