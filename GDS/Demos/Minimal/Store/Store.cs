using UnityEngine;
using GDS.Sample;
using GDS.Core;
using GDS.Core.Events;
using static GDS.Core.LogUtil;
using static GDS.Core.InventoryFactory;
using static GDS.Core.InventoryExtensions;
using static GDS.Sample.ItemFactory;

namespace GDS.Minimal {

    /// <summary>
    /// `Store` is a singleton that contains all the system state, listens to UI events and updates the state
    /// Only the Store can modify it's internal state
    /// </summary>
    public class Store {
        public Store() {
            // Listen (subscribe) to reset event - it is triggered when entering play mode
            EventBus.GlobalBus.Subscribe<ResetEvent>(e => Reset());
            // Listen to pick and place UI events
            Bus.Subscribe<PickItemEvent>(e => OnPickItem(e as PickItemEvent));
            Bus.Subscribe<PlaceItemEvent>(e => OnPlaceItem(e as PlaceItemEvent));
            // Initilize by calling Reset
            Reset();
        }

        // Store singleton instance
        public static readonly Store Instance = new();
        // Event bus - the channel used to pass all the events
        public readonly EventBus Bus = new();
        // Main inventory state
        public readonly ListBag MainInventory = CreateListBag("MainInventory", 40);
        // DraggedItem contains info about the item being dragged (not a reference)
        // Views and behaviors subscribe to this and react by re-rendering or triggering other flows
        public readonly Observable<Item> DraggedItem = new(Item.NoItem);

        /// <summary>
        /// Resets the Store state
        /// Sets the inventory state by creating items and adding them to the list
        /// </summary>
        void Reset() {
            Debug.Log($"Reseting ".Yellow() + "[Basic Store]".Gray());
            MainInventory.SetState(
                Create(BaseId.WarriorHelmet, Rarity.Unique),
                Create(BaseId.LeatherArmor, Rarity.Magic),
                Create(BaseId.SteelBoots, Rarity.Rare),
                Create(BaseId.ShortSword, Rarity.Common),
                Create(BaseId.Apple, Rarity.Rare, 10),
                Create(BaseId.Wood, Rarity.NoRarity, 199)
            );
        }

        /// <summary>
        /// PickItem event handler
        /// If picking is successful, sets the new state of Dragged Item and notifies the 
        /// inventory from which the item was picked
        /// </summary>
        /// <param name="e"></param>
        void OnPickItem(PickItemEvent e) {
            LogEvent(e);
            var (success, replacedItem) = e.Bag.PickItem(e.Item, e.Slot);
            if (!success) return;
            DraggedItem.SetValue(replacedItem);
            e.Bag.Notify();
        }

        /// <summary>
        /// PlaceItem event handler
        /// If placing is successful, sets the new state of the Dragged Item and notifies the
        /// inventory in which the item was placed
        /// </summary>
        /// <param name="e"></param>
        void OnPlaceItem(PlaceItemEvent e) {
            LogEvent(e);
            var (success, replacedItem) = e.Bag.PlaceItem(e.Item, e.Slot);
            if (!success) return;
            DraggedItem.SetValue(replacedItem);
            e.Bag.Notify();
        }
    }
}