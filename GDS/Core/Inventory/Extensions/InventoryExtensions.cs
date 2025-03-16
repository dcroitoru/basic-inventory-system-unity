#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using static GDS.Core.InventoryFactory;
using UnityEngine;

// This file contains extension methods related to inventories (bags), slots and items
namespace GDS.Core {
    public static class InventoryExtensions {

        public static string Name(this Item item) => item.ItemBase.Name;
        public static int Quant(this Item item) => item.ItemData.Quant;
        public static Item Clone(this Item item) => item with { Id = Id() };
        public static Item Clone(this Item item, ItemData itemData) => item with { Id = Id(), ItemData = itemData };
        public static bool CanStack(Item item, Item otherItem) => item.ItemBase.Stackable && item.ItemBase == otherItem.ItemBase;
        public static T WithQuant<T>(this T item, int quant) where T : Item => item with { ItemData = item.ItemData with { Quant = quant } };
        public static T IncQuant<T>(this T item) where T : Item => item.WithQuant(item.ItemData.Quant + 1);
        public static T DecQuant<T>(this T item) where T : Item => item.ItemData.Quant > 0 ? item.WithQuant(item.ItemData.Quant - 1) : item;

        // Can unstack if
        // • bag and slot accept item
        // • source is stackable
        // • both are the same base OR target is NoItem
        // • source quant is > 1 (the case where quant is 1 is treated by `place`)
        // • target quant + 1 < max (not aplicable yet, since quant has no max)
        public static bool CanUnstackTo(this Item source, Bag bag, Slot slot) =>
            bag.Accepts(source)
            && slot.Accepts(source)
            && source.ItemBase.Stackable
            && (source.ItemBase == slot.Item.ItemBase || slot.IsEmpty())
            && source.Quant() > 1;

        public static Size Size(this Item item) => item.ItemData switch {
            _ => item.ItemBase.Size
        };

        public static bool IsEmpty(this Slot slot) => slot.Item == Item.NoItem;
        public static bool IsNotEmpty(this Slot slot) => slot.Item != Item.NoItem;

        public static bool IsItem(this Item item) => item != Item.NoItem;
        public static bool IsNoItem(this Item item) => item == Item.NoItem;

        public static bool IsEmpty(this ListBag bag) => bag.Slots.All(IsEmpty);
        public static bool IsEmpty(this Bag bag) => bag switch {
            ListBag b => b.IsEmpty(),
            _ => false
        };
        public static bool IsFull(this ListBag bag) => bag.Slots.All(IsNotEmpty);

        public static bool IsFull(this Bag bag) => bag switch {
            ListBag b => b.IsFull(),
            _ => false
        };



        public static int ToIndex(this Pos pos, Size size) => pos.Y * size.W + pos.X;

        public static bool IsItemInBounds(Size bounds, Size size, Pos pos)
            => pos.X >= 0
            && pos.Y >= 0
            && size.W + pos.X <= bounds.W
            && size.H + pos.Y <= bounds.H;

        public static int GetNextEmptyIndex(this ListBag bag) => bag.Slots.FindIndex((x) => IsEmpty(x));

        public static Slot Clear(this Slot slot) => slot with { Item = Item.NoItem };

        public static T Clear<T>(this T slot) where T : Slot => slot with { Item = Item.NoItem };

        public static Slot SetItem(this Slot slot, Item item) => slot with { Item = item };

        public static IEnumerable<ListSlot> EmptySlots<T>(this T bag) where T : ListBag => bag.Slots.Where(IsEmpty);

        public static IEnumerable<ListSlot> NonEmptySlots<T>(this T bag) where T : ListBag => bag.Slots.Where(IsNotEmpty);

        public static IEnumerable<Item> NotEmpty(this IEnumerable<Item> items) => items.Where(IsItem).ToList();


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

        public static bool AddItem(this Bag bag, Item item) {
            return bag switch {
                ListBag b => b.AddItem(item),
                _ => false
            };
        }

        /// <summary>
        /// Adds an item to a List bag
        /// </summary>
        /// <returns>True if item was added, false otherwise</returns>
        static bool AddItem(this ListBag bag, Item item) {
            var index = bag.GetNextEmptyIndex();
            if (index == -1) return false;
            bag.Slots[index] = bag.Slots[index] with { Item = item };
            bag.Data.Notify();
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
            if (!bag.Slots.Contains(slot)) { Debug.LogWarning("Did not find slot " + slot.Index.ToString().Orange()); return false; }
            bag.Slots[slot.Index] = bag.Slots[slot.Index] with { Item = item };
            bag.Data.Notify();
            return true;
        }





        /// <summary>
        /// Adds items until the bag is full
        /// </summary>
        /// <returns>Items that do not fit</returns>
        public static IEnumerable<Item> AddItems(this Bag bag, params Item[] items) {
            return bag switch {
                ListBag b => AddItems(b, items),
                _ => items
            };
        }

        static IEnumerable<Item> AddItems(this ListBag bag, params Item[] items) {
            if (bag.IsFull()) return items;
            var itemsAddedResult = items.Select(bag.AddItem).ToList();
            var itemsThatDontFit = items.Where((_, index) => itemsAddedResult[index] == false);
            bag.Data.Notify();
            return itemsThatDontFit;
        }


        /// <summary>
        /// Removes an item from a bag
        /// </summary>
        /// <returns>True and the item if the item was removed from the bag, otherwise false and NoItem </returns>
        public static (bool, Item) PickItem(this Bag bag, Item item, Slot slot) {
            if (item == Item.NoItem) return (false, Item.NoItem);
            return (bag, item, slot) switch {
                (DenseListBag b, _, ListSlot s) => PickItem(b, item, s),
                (ListBag b, _, ListSlot s) => PickItem(b, item, s),
                (SetBag b, _, SetSlot s) => PickItem(b, item, s),
                _ => (false, Item.NoItem)
            };
        }

        /// <summary>
        /// Same as `PickItem` except it also sets dragged item on success
        /// </summary>
        /// <returns></returns>
        public static (bool, Item) PickItem(this Bag bag, Item item, Slot slot, Observable<Item> draggedItem) {
            // Debug.Log($"should pick item from bag {item}");
            if (item == Item.NoItem) return (false, Item.NoItem);
            var (success, replacedItem) = PickItem(bag, item, slot);
            if (success) draggedItem.SetValue(replacedItem);
            return (success, replacedItem);
        }

        static (bool, Item) PickItem(this DenseListBag bag, Item item, ListSlot slot) {
            var index = bag.Slots.FindIndex(s => s == slot);
            bag.Slots.RemoveAt(index);
            bag.Slots.Add(slot.Clear());
            bag.Data.Notify();
            return (true, item);
        }

        static (bool, Item) PickItem(this ListBag bag, Item item, ListSlot slot) {
            bag.Slots[slot.Index] = bag.Slots[slot.Index] with { Item = Item.NoItem };
            bag.Data.Notify();
            return (true, item);
        }

        static (bool, Item) PickItem(this SetBag bag, Item item, SetSlot slot) {
            bag.Slots[slot.Key] = bag.Slots[slot.Key] with { Item = Item.NoItem };
            bag.Data.Notify();
            return (true, item);
        }


        public static bool UnstackItem(this Bag bag, Item item, Slot slot, Observable<Item> dragged) {
            if (!item.ItemBase.Stackable || item.ItemData.Quant <= 1) return false;
            int half = item.ItemData.Quant / 2;
            var newDraggedItem = item.Clone(item.ItemData with { Quant = half });
            var newBagItem = item.WithQuant(item.ItemData.Quant - half);
            dragged.SetValue(newDraggedItem);
            bag.SetItem(slot, newBagItem);
            return true;
        }


        public static bool UnstackDraggedItem(this Bag bag, Slot slot, Observable<Item> dragged) {
            if (!dragged.Value.CanUnstackTo(bag, slot)) return false;
            var newDraggedItem = dragged.Value.DecQuant();
            var newBagItem = slot.IsEmpty() ? dragged.Value.Clone().WithQuant(1) : slot.Item.IncQuant();
            dragged.SetValue(newDraggedItem);
            bag.SetItem(slot, newBagItem);
            return true;
        }

        /// <summary>
        /// Removes an item
        /// </summary>
        /// <returns><c>true</c> if item was removed, <c>false</c> otherwise</returns>
        public static bool RemoveItem(this Bag bag, Item item) {
            return bag switch {
                DenseListBag b => RemoveItem(b, item),
                ListBag b => RemoveItem(b, item),
                _ => false
            };
        }

        static bool RemoveItem(this DenseListBag bag, Item item) {
            var index = bag.Slots.FindIndex(slot => slot.Item == item);
            if (index == -1) return false;
            var slot = bag.Slots.ElementAt(index);
            bag.Slots.RemoveAt(index);
            bag.Slots.Add(slot.Clear());
            bag.Data.Notify();
            return true;
        }

        static bool RemoveItem(this ListBag bag, Item item) {
            var index = bag.Slots.FindIndex(slot => slot.Item == item);
            if (index == -1) return false;
            bag.Slots[index] = bag.Slots[index] with { Item = Item.NoItem };
            bag.Data.Notify();
            return true;
        }

        /// <summary>
        /// Places an item in a slot belonging to a bag. Will stack items if possible (same base and stackable).
        /// </summary>
        /// <returns><para><c>(true, Item)</c> if placement was successful, where Item is the replaced item (previous item in the slot or NoItem if empty)</para>
        /// <para><c>(false, NoItem)</c> otherwise</para>
        /// </returns>
        public static (bool, Item) PlaceItem(this Bag bag, Item item, Slot slot) {
            return (bag, slot) switch {
                (SetBag b, SetSlot s) => PlaceItem(b, item, s),
                (ListBag b, ListSlot s) => PlaceItem(b, item, s),
                _ => (false, Item.NoItem)
            };
        }

        /// <summary>
        /// Same as `PlaceItem` except it also sets dragged item on success 
        /// </summary>
        /// <param name="bag"></param>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <param name="draggedItem"></param>
        /// <returns></returns>
        public static (bool, Item) PlaceItem(this Bag bag, Item item, Slot slot, Observable<Item> draggedItem) {
            var (success, replacedItem) = PlaceItem(bag, item, slot);
            if (success) draggedItem.SetValue(replacedItem);
            return (success, replacedItem);
        }

        static (bool, Item) PlaceItem(this SetBag bag, Item item, SetSlot slot) {
            if (!bag.Accepts(item)) return (false, Item.NoItem);
            if (!slot.Accepts(item)) return (false, Item.NoItem);

            var hasKey = bag.Slots.ContainsKey(slot.Key);
            if (!hasKey) return (false, Item.NoItem);

            bag.Slots[slot.Key] = bag.Slots[slot.Key] with { Item = item };
            bag.Data.Notify();
            return (true, slot.Item);
        }

        static (bool, Item) PlaceItem(this ListBag bag, Item item, ListSlot slot) {
            if (!bag.Accepts(item)) return (false, Item.NoItem);
            if (!slot.Accepts(item)) return (false, Item.NoItem);

            var index = slot.Index;
            if (CanStack(item, slot.Item)) {
                var newQuant = item.ItemData.Quant + slot.Item.ItemData.Quant;
                bag.Slots[index] = bag.Slots[index] with { Item = bag.Slots[index].Item.WithQuant(newQuant) };
                bag.Data.Notify();
                return (true, Item.NoItem);
            }

            bag.Slots[index] = bag.Slots[index] with { Item = item };
            bag.Data.Notify();
            return (true, slot.Item);
        }

        /// <summary>
        /// Moves an item from one bag to another
        /// </summary>
        /// <returns>True if move was successful, False otherwise (for example bag could be full or 
        /// not accept the item)</returns>
        public static bool MoveItem(this Bag fromBag, Bag toBag, Item item) {
            return (fromBag, toBag) switch {
                (DenseListBag b1, ListBag b2) => MoveItem(b1, b2, item),
                (ListBag b1, ListBag b2) => MoveItem(b1, b2, item),
                _ => false
            };
        }

        static bool MoveItem(this DenseListBag fromBag, ListBag toBag, Item item) {
            return toBag.Accepts(item) && AddItem(toBag, item) && RemoveItem(fromBag, item);
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
            var newItem = slot.Item.DecQuant();
            bag.Slots[index] = slot with { Item = newItem.Quant() == 0 ? Item.NoItem : newItem };
            bag.Data.Notify();
            return (true, newItem);
        }

        /// <summary>
        /// Sets the state of a SetBag
        /// </summary>        
        public static SetBag SetState(this SetBag bag, params (string, Item)[] items) {
            bag.Clear();
            foreach (var (key, item) in items) bag.SetItem(key, item);
            bag.Data.Notify();
            return bag;
        }

        /// <summary>
        /// Sets the state of a ListBag
        /// </summary>
        /// <returns></returns>
        public static ListBag SetState(this ListBag bag, params Item[] items) {
            bag.Clear();
            bag.AddItems(items);
            // bag.Data.Notify();
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




