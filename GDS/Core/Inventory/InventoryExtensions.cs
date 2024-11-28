#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This file contains extension methods related to inventories (bags), slots and items
namespace GDS.Core {
    public static class InventoryExtensions {

        public static bool IsEmpty(this Slot slot) => slot.Item is NoItem;

        public static bool IsNotEmpty(this Slot slot) => !slot.IsEmpty();

        public static bool IsNotEmpty(this Item item) => item is not NoItem;

        public static bool IsEmpty(this ListBag inventory) => inventory.Slots.All(IsEmpty);

        public static bool IsFull(this ListBag inventory) => inventory.Slots.All(IsNotEmpty);

        public static int ToIndex(this Pos pos, Size size) => pos.Y * size.W + pos.X;

        public static int GetNextEmptyIndex(this ListBag bag) => bag.Slots.FindIndex((x) => IsEmpty(x));

        public static Slot Clear(this Slot slot) => slot with { Item = Item.NoItem };

        public static Slot SetItem(this Slot slot, Item item) => slot with { Item = item };

        public static IEnumerable<ListSlot> EmptySlots<T>(this T bag) where T : ListBag => bag.Slots.Where(IsEmpty);

        public static IEnumerable<ListSlot> NonEmptySlots<T>(this T bag) where T : ListBag => bag.Slots.Where(IsNotEmpty);

        public static IEnumerable<Item> NotEmpty(this IEnumerable<Item> items) => items.Where(IsNotEmpty).ToList();

        public static T AcceptsFn<T>(this T bag, FilterFn accepts) where T : Bag {
            bag.Accepts = accepts;
            return bag;
        }

        /// <summary>
        /// Cleares a ListBag
        /// </summary>
        public static void Clear(this ListBag bag) {
            for (int i = 0; i < bag.Slots.Count; i++) bag.Slots[i] = bag.Slots[i] with { Item = Item.NoItem };
        }

        /// <summary>
        /// Cleares a SetBag
        /// </summary>
        public static void Clear(this SetBag bag) {
            string key;
            for (int i = 0; i < bag.Slots.Count; i++) {
                key = bag.Slots.Keys.ElementAt(i);
                bag.Slots[key] = bag.Slots[key] with { Item = Item.NoItem };
            }
        }

        /// <summary>
        /// Adds an item to a List bag
        /// </summary>
        /// <returns>True if item was added, false otherwise</returns>
        public static bool AddItem(this ListBag bag, Item item) {
            var index = bag.GetNextEmptyIndex();
            if (index == -1) return false;
            bag.Slots[index] = bag.Slots[index] with { Item = item };

            return true;
        }

        public static bool SetItem(this Bag bag, Slot slot, Item item) {
            return (bag, slot) switch {
                (ListBag b, ListSlot s) => b.SetItem(s, item),
                _ => false
            };
        }

        public static bool SetItem(this SetBag bag, string slotKey, Item item) {
            if (!bag.Slots.ContainsKey(slotKey)) {
                Debug.LogWarning("Did not find slot " + slotKey.Orange());
                return false;
            }

            bag.Slots[slotKey] = bag.Slots[slotKey] with { Item = item };
            return true;
        }

        static bool SetItem(this ListBag bag, ListSlot slot, Item item) {
            if (!bag.Slots.Contains(slot)) {
                Debug.LogWarning("Did not find slot " + slot.Index.ToString().Orange());
                return false;
            }

            bag.Slots[slot.Index] = bag.Slots[slot.Index] with { Item = item };
            return true;
        }



        /// <summary>
        /// Adds items until the bag is full
        /// </summary>
        /// <returns>Items that do not fit</returns>
        public static IEnumerable<Item> AddItems(this Bag bag, params Item[] items) {
            return bag switch {
                ListBag => AddItems((ListBag)bag, items),
                _ => items
            };
        }

        static IEnumerable<Item> AddItems(this ListBag bag, params Item[] items) {
            if (bag.IsFull()) return items;
            var itemsAddedResult = items.Select(bag.AddItem).ToList();
            var itemsThatDontFit = items.Where((_, index) => itemsAddedResult[index] == false);
            return itemsThatDontFit;
        }

        /// <summary>
        /// Removes an item from a bag
        /// </summary>
        /// <returns>True and the item if the item was removed from the bag, otherwise false and NoItem </returns>
        public static (bool, Item) PickItem(this Bag bag, Item item, Slot slot) {
            // Debug.Log($"should pick item from bag {item}");
            if (item == Item.NoItem) return (false, Item.NoItem);
            return (bag, item, slot) switch {
                (ListBag b, _, ListSlot s) => PickItem(b, item, s),
                (SetBag b, _, SetSlot s) => PickItem(b, item, s),
                _ => (false, Item.NoItem)
            };
        }

        static (bool, Item) PickItem(this ListBag bag, Item item, ListSlot slot) {

            bag.Slots[slot.Index] = bag.Slots[slot.Index] with { Item = Item.NoItem };
            return (true, item);
        }

        static (bool, Item) PickItem(this SetBag bag, Item item, SetSlot slot) {
            bag.Slots[slot.Key] = bag.Slots[slot.Key] with { Item = Item.NoItem };
            return (true, item);
        }

        /// <summary>
        /// Removes an item
        /// </summary>
        /// <returns>True if item was removed, False otherwise</returns>
        public static bool RemoveItem(this Bag bag, Item item) {
            return bag switch {
                ListBag b => RemoveItem(b, item),
                _ => false
            };
        }

        static bool RemoveItem(this ListBag bag, Item item) {
            var index = bag.Slots.FindIndex(slot => slot.Item == item);
            if (index == -1) return false;
            bag.Slots[index] = bag.Slots[index] with { Item = Item.NoItem };
            return true;
        }

        /// <summary>
        /// Places an item in a slot belonging to a bag. Will stack items if possible (both items of the same type and stackable).
        /// </summary>
        /// <returns>True and the replaced item (item that was in the slot before this, NoItem if slot was empty)
        /// if placement was successful
        /// False and NoItem otherwise
        /// </returns>
        public static (bool, Item) PlaceItem(this Bag bag, Item item, Slot slot) {
            return (bag, slot) switch {
                (SetBag b, SetSlot s) => PlaceItem(b, item, s),
                (ListBag b, ListSlot s) => PlaceItem(b, item, s),
                _ => (false, Item.NoItem)
            };
        }

        static (bool, Item) PlaceItem(this SetBag bag, Item item, SetSlot slot) {
            if (!bag.Accepts(item)) return (false, Item.NoItem);
            if (!slot.Accepts(item)) return (false, Item.NoItem);

            var hasKey = bag.Slots.ContainsKey(slot.Key);
            if (!hasKey) return (false, Item.NoItem);

            bag.Slots[slot.Key] = bag.Slots[slot.Key] with { Item = item };
            return (true, slot.Item);
        }

        static (bool, Item) PlaceItem(this ListBag bag, Item item, ListSlot slot) {
            if (!bag.Accepts(item)) return (false, Item.NoItem);
            if (!slot.Accepts(item)) return (false, Item.NoItem);

            var index = slot.Index;
            if (ItemExt.CanStack(item, slot.Item)) {
                var newQuant = item.ItemData.Quant + slot.Item.ItemData.Quant;
                bag.Slots[index] = bag.Slots[index] with { Item = bag.Slots[index].Item.SetQuant(newQuant) };
                return (true, Item.NoItem);
            }

            bag.Slots[index] = bag.Slots[index] with { Item = item };
            return (true, slot.Item);
        }

        /// <summary>
        /// Moves an item from one bag to another
        /// </summary>
        /// <returns>True if move was successful, False otherwise (for example bag could be full or 
        /// not accept the item)</returns>
        public static bool MoveItem(this Bag fromBag, Bag toBag, Item item) {
            return (fromBag, toBag) switch {
                (ListBag b1, ListBag b2) => MoveItem(b1, b2, item),
                _ => false
            };
        }

        static bool MoveItem(this ListBag fromBag, ListBag toBag, Item item) {
            return toBag.Accepts(item) && AddItem(toBag, item) && RemoveItem(fromBag, item);
        }

        /// <summary>
        /// Consumes an item at a position by decrementing its quantity
        /// If new quantity is 0, the slot is cleared (the item becomes a NoItem)
        /// </summary>
        /// <returns>True and the item if an item was consumed, False and NoItem otherwise</returns>
        public static (bool, Item) Consume(this ListBag bag, int index) {
            if (index < 0 || index >= bag.Slots.Count) return (false, Item.NoItem);
            var slot = bag.Slots[index];
            if (slot.IsEmpty()) return (false, Item.NoItem);
            var item = slot.Item;
            var newQuant = slot.Item.ItemData.Quant - 1;
            var newItem = newQuant == 0 ? Item.NoItem : slot.Item with { ItemData = new(newQuant) };
            bag.Slots[index] = slot with { Item = newItem };
            return (true, item);
        }

        /// <summary>
        /// Sets the state of a SetBag
        /// </summary>        
        public static SetBag SetState(this SetBag bag, params (string, Item)[] items) {
            bag.Clear();
            foreach (var (key, item) in items) bag.SetItem(key, item);
            bag.Notify();
            return bag;
        }

        /// <summary>
        /// Sets the state of a ListBag
        /// </summary>
        /// <returns></returns>
        public static ListBag SetState(this ListBag bag, params Item[] items) {
            bag.Clear();
            bag.AddItems(items);
            bag.Notify();
            return bag;
        }

        public static Bag SetState(this Bag bag, params Item[] items) {
            return bag switch {
                ListBag listBag => listBag.SetState(items),
                _ => bag
            };
        }

    }

}