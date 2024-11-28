using System;
using System.Linq;
namespace GDS.Core {
    /// <summary>
    /// Contains factory methods for creating all types of bags and slots
    /// </summary>
    public static class InventoryFactory {
        public static Slot NoSlot = new(Item.NoItem);
        public static Slot CreateSlot(Item item) => new Slot(item);
        public static Slot CreateSlot() => CreateSlot(Item.NoItem);

        public static ListSlot CreateListSlot(int index) => new ListSlot(index, Item.NoItem);
        public static SetSlot CreateSetSlot(string key) => new SetSlot(key, Item.NoItem);

        public static ListBag CreateListBag(string id, int size) {
            var slots = Enumerable.Range(0, size).Select(CreateListSlot).ToList();
            var bag = new ListBag(id, slots, size);
            return bag;
        }

        public static T CreateListBag<T>(string id, int size) where T : ListBag {
            var slots = Enumerable.Range(0, size).Select(CreateListSlot).ToList();
            var bag = (T)Activator.CreateInstance(typeof(T), id, slots, size);
            return bag;
        }

        public static SetBag CreateSetBag(string id, SetSlot[] slots) {
            var slotsDict = slots.ToDictionary(slot => slot.Key, slot => slot);
            var bag = new SetBag(id, slotsDict);
            return bag;
        }

        public static SetBag CreateSetBag(string id, string[] slotKeys) {
            var slots = slotKeys.ToDictionary(key => key, key => CreateSetSlot(key));
            var bag = new SetBag(id, slots);
            return bag;
        }
    }
}