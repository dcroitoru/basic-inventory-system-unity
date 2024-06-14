#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static GDS.Factory;
using static GDS.Util;
using static System.Diagnostics.Debug;


namespace GDS {


    public static class Fn {

        public static bool Accepts(this Slot slot, Item item) => DB.Accepts.GetValueOrDefault(slot.Type)?.Contains(item.Class) ?? true;

        public static bool IsEmpty(this Slot slot) => slot.Item == Factory.NoItem;

        public static bool IsNotEmpty(this Slot slot) => !slot.IsEmpty();

        public static bool IsFull(this Inventory inventory) => inventory.Slots.All(IsNotEmpty);

        public static bool IsEmpty(this Inventory inventory) => inventory.Slots.All(IsEmpty);

        public static bool IsStackable(this Item item) => DB.Stackable.Contains(item.Type);


        public static bool CanPlace(this SlotType slotType, Item item) => slotType switch {
            SlotType.Default => true,
            SlotType.RemoveOnly => false,
            _ => DB.Accepts.GetValueOrDefault(slotType)?.Contains(item.Class) ?? false
        };

        public static int GetNextEmptyIndex(this Inventory bag) => bag.Slots.FindIndex((x) => IsEmpty(x));

        public static Slot Clear(this Slot slot) => slot with { Item = NoItem };

        public static Slot SetItem(this Slot slot, Item item) => slot with { Item = item };



        /**************************************************************
        * Impure Functions
        **************************************************************/


        /**************************************************************
        * Add items
        **************************************************************/


        // adds item to bag
        // returns true if added; false otherwise
        static bool AddItem(this Inventory bag, Item item) {
            var index = bag.GetNextEmptyIndex();
            if (index == -1) return false;

            bag.Slots[index] = bag.Slots[index] with { Item = item };

            Assert(bag.Slots.Any(slot => slot.Item == item));
            return true;
        }

        // adds items one by one to bag until bag is full
        // returns items that do not fit        
        public static IEnumerable<Item> AddItems(this Bag bag, params Item[] items) {
            return bag switch {
                Inventory => AddItems((Inventory)bag, items),
                _ => items
            };
        }

        public static IEnumerable<Item> AddItems(this Inventory bag, params Item[] items) {

            if (bag.IsFull()) return items;

            var itemsAddedResult = items.Select(bag.AddItem).ToList();
            var itemsThatDontFit = items.Where((_, index) => itemsAddedResult[index] == false);

            return itemsThatDontFit;
        }

        /************************************************************** 
        * Pick item
        ***************************************************************/
        // removes an item from bag
        // returns (true/false if item was picked, the item)
        public static (bool, Item) PickItem(this Bag bag, Item item) {
            if (item == NoItem) return (false, NoItem);
            return bag switch {
                Equipment => PickItem((Equipment)bag, item),
                Inventory => PickItem((Inventory)bag, item),
                _ => (false, NoItem)
            };
        }

        static (bool, Item) PickItem(this Inventory bag, Item item) {
            var index = bag.Slots.FindIndex(slot => slot.Item == item);
            if (index == -1) return (false, NoItem);

            bag.Slots[index] = bag.Slots[index] with { Item = NoItem };
            return (true, item);
        }

        static (bool, Item) PickItem(this Equipment bag, Item item) {
            var slot = bag.Slots.Values.FirstOrDefault(slot => slot.Item == item);
            if (slot == null) return (false, NoItem);

            bag.Slots[slot.Type] = bag.Slots[slot.Type] with { Item = NoItem };
            return (true, item);
        }

        /************************************************************** 
        * Place item
        ***************************************************************/
        public static (bool, Item) PlaceItem(this Bag bag, Item item, Slot slot) {
            return bag switch {
                Equipment => PlaceItem((Equipment)bag, item, slot),
                Inventory => PlaceItem((Inventory)bag, item, slot),
                _ => (false, NoItem)
            };
        }

        static (bool, Item) PlaceItem(this Equipment bag, Item item, Slot slot) {
            if (!CanPlace(slot.Type, item)) return (false, NoItem);

            var hasKey = bag.Slots.ContainsKey(slot.Type);
            if (!hasKey) return (false, NoItem);

            bag.Slots[slot.Type] = bag.Slots[slot.Type] with { Item = item };
            return (true, slot.Item);
        }

        static (bool, Item) PlaceItem(this Inventory bag, Item item, Slot slot) {
            if (!CanPlace(slot.Type, item)) return (false, NoItem);

            var index = bag.Slots.IndexOf(slot);
            if (index == -1) return (false, NoItem);

            if (item.IsStackable() && slot.Item.Type == item.Type) {
                Log("should stack");
                var newQuant = item.Quant + slot.Item.Quant;
                var newItem = CreateItem(item.Type, newQuant);
                bag.Slots[index] = bag.Slots[index] with { Item = newItem };

                return (true, NoItem);
            }

            bag.Slots[index] = bag.Slots[index] with { Item = item };
            return (true, slot.Item);
        }




        /**
        * Consume
        */
        public static (bool, Item) Consume(this Hotbar bag, int index) {
            Log("should consume", bag.Slots, index);
            if (index < 0 || index >= bag.Slots.Count) return (false, NoItem);
            var slot = bag.Slots[index];
            if (slot.IsEmpty()) return (false, NoItem);
            var item = slot.Item;
            var newQuant = slot.Item.Quant - 1;
            var newItem = newQuant == 0 ? NoItem : slot.Item with { Quant = newQuant };
            bag.Slots[index] = slot with { Item = newItem };
            return (true, item);
        }

        /**
        * Set state
        */

        public static void SetState(this Equipment bag, List<SlotItemDTO> dto) {
            var keys = bag.Slots.Keys.ToHashSet();
            var slots = bag.Slots;
            foreach (var key in keys) slots[key] = slots[key] with { Item = NoItem };
            dto.ForEach(slot => slots[slot.pos] = slots[slot.pos] with { Item = CreateItem(slot.item) });

        }

        public static void SetState(this Inventory bag, List<ListItemDTO> dto) {
            var slots = bag.Slots;
            for (var i = 0; i < slots.Count(); i++) slots[i] = slots[i] with { Item = NoItem };
            dto.ForEach(slot => slots[slot.pos] = slots[slot.pos] with { Item = CreateItem(slot.item) });
        }

        public static void SetState(this Chest bag, List<ItemDTO> dto) {
            var slots = bag.Slots;
            slots.Clear();
            dto.ForEach(slot => slots.Add(CreateSlot(SlotType.RemoveOnly, CreateItem(slot))));
        }

        public static void SetState(this Chest bag, List<Item> items) {
            var newChest = CreateChest(bag.Id, items.ToArray());
            bag.Slots.Clear();
            newChest.Slots.ForEach(slot => bag.Slots.Add(slot));
        }
    }
}