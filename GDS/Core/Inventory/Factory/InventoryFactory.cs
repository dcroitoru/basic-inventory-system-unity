using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace GDS.Core {
    /// <summary>
    /// Contains factory methods for creating all types of bags and slots
    /// </summary>
    public static class InventoryFactory {
        static int _lastId = 0;
        public static int NextId() => _lastId + 1;
        public static int Id() => _lastId++;

        public static ItemBase CreateBase(string id, string name, string iconPath) => new(id, name, iconPath, false, Sizes.Size1x1);
        public static ItemBase CreateBase(string id, string name, string iconPath, bool stackable) => new(id, name, iconPath, stackable, Sizes.Size1x1);

        public static Item Create(ItemBase itemBase, ItemData itemData) => new(Id(), itemBase, itemData);
        public static Item Create(ItemBase itemBase, int quant) => new(Id(), itemBase, new(quant));
        public static Item Create(ItemBase itemBase) => new(Id(), itemBase, ItemData.NoItemData);


        public static Slot CreateSlot(Item item) => new Slot(item);
        public static Slot CreateSlot() => CreateSlot(Item.NoItem);

        public static ListSlot CreateListSlot(int index) => new ListSlot(index, Item.NoItem);
        public static ListSlot CreateListSlot(int index, Core.FilterFn filter) => new ListSlot(index, Item.NoItem) { Accepts = filter };
        public static SetSlot CreateSetSlot(string key) => new SetSlot(key, Item.NoItem);
        public static SetSlot CreateSetSlot(string key, FilterFn accepts) => new SetSlot(key, Item.NoItem) { Accepts = accepts };

        public static T CreateListBag<T>(string id, List<ListSlot> slots) where T : ListBag {
            var bag = (T)Activator.CreateInstance(typeof(T), id, slots.Count, new Observable<List<ListSlot>>(slots));
            return bag;
        }
        public static ListBag CreateListBag(string id, int size) {
            var slots = Enumerable.Range(0, size).Select(CreateListSlot).ToList();
            var bag = new ListBag(id, size, new(slots));
            return bag;
        }

        public static ListBag CreateListBag(string id, params FilterFn[] slotFilters) {
            var slots = slotFilters.Select((fn, index) => CreateListSlot(index, fn)).ToList();
            var bag = new ListBag(id, slots.Count, new(slots));
            return bag;
        }

        public static T CreateListBag<T>(string id, int size) where T : ListBag {
            var slots = Enumerable.Range(0, size).Select(CreateListSlot).ToList();
            var bag = (T)Activator.CreateInstance(typeof(T), id, size, new Observable<List<ListSlot>>(slots));
            return bag;
        }

        public static SetBag CreateSetBag(string id, SetSlot[] slots) {
            var slotsDict = slots.ToDictionary(slot => slot.Key, slot => slot);
            var bag = new SetBag(id, new(slotsDict));
            return bag;
        }

        public static SetBag CreateSetBag(string id, string[] slotKeys) {
            var slots = slotKeys.ToDictionary(key => key, key => CreateSetSlot(key));
            var bag = new SetBag(id, new(slots));
            return bag;
        }
    }
}