
using GDS.Core;
using GDS.Core.Events;
using static GDS.Core.LogUtil;
using static GDS.Core.InventoryFactory;
using static GDS.Core.InventoryExtensions;

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


        ItemBase WarriorHelmet = CreateBase("WarriorHelmet", "Warrior Helmet", "Shared/Images/items/helmet");
        ItemBase LeatherArmor = CreateBase("LeatherArmor", "Leaher Armor", "Shared/Images/items/armor");
        ItemBase Apple = CreateBase("Apple", "Apple", "Shared/Images/items/apple", true);
        ItemBase Wood = CreateBase("Wood", "Wood", "Shared/Images/items/wood", true);

        /// <summary>
        /// Resets the Store state
        /// Sets the inventory state by creating items and adding them to the list
        /// </summary>
        void Reset() {
            // Debug.Log($"Reseting ".Yellow() + "[Minimal Store]".Gray());

            MainInventory.Clear();
            MainInventory.AddItems(
                Create(WarriorHelmet),
                Create(LeatherArmor),
                Create(Apple),
                Create(Apple, 5),
                Create(Wood, 50),
                Create(Wood, 100)
            );
        }

        /// <summary>
        /// PickItem event handler
        /// If picking is successful, updates the inventory and dragged item
        /// </summary>
        /// <param name="e"></param>
        void OnPickItem(PickItemEvent e) {
            LogEvent(e);
            e.Bag.PickItem(e.Item, e.Slot, DraggedItem);
        }

        /// <summary>
        /// PlaceItem event handler
        /// If placing is successful, updates the inventory and dragged item
        /// </summary>
        /// <param name="e"></param>
        void OnPlaceItem(PlaceItemEvent e) {
            LogEvent(e);
            e.Bag.PlaceItem(e.Item, e.Slot, DraggedItem);
        }
    }
}