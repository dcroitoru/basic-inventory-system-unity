using System;
using System.Collections.Generic;
using System.Linq;
using static GDS.Util;
namespace GDS {

    public static class Factory {
        public static NoBag NoBag = new NoBag("");
        public static Item NoItem = new Item(new(-1), ItemType.NoItem, 0, new Size(0, 0), ItemClass.NoClass);

        static int lastId = 0;
        public static ItemId CreateItemId() => new ItemId(lastId++);
        public static SlotId CreateSlotId() => new SlotId(lastId++);



        public static Item CreateItem(ItemId id, ItemType type, int quant) {
            var cls = DB.Classes.GetValueOrDefault(type);
            var size = DB.Sizes.GetValueOrDefault(cls) ?? new(1, 1);
            return new Item(id, type, quant, size, cls);
        }
        public static Item CreateItem(ItemType type, int quant) => CreateItem(CreateItemId(), type, quant);
        public static Item CreateItem(ItemType type) => CreateItem(type, 1);
        public static Item CreateItem(ItemDTO dto) {
            var (id, type, quant) = dto;
            return CreateItem(new ItemId(id), type, quant);
        }

        public static Slot CreateSlot(SlotType slotType, Item item) => new Slot(CreateSlotId(), slotType, item);
        public static Slot CreateSlot(SlotType slotType) => CreateSlot(slotType, NoItem);
        public static Slot CreateSlot() => CreateSlot(SlotType.Default, NoItem);

        public static List<Slot> CreateSlots(int size, SlotType type) => Enumerable.Range(0, size).Select(x => CreateSlot(type)).ToList();
        public static List<Slot> CreateSlots(int size) => CreateSlots(size, SlotType.Default);

        public static Inventory CreateInventory(int size = 10, string id = "inventory") => new Inventory(id, size, CreateSlots(size));


        public static Hotbar CreateHotbar(int Size, string id = "hotbar") => new Hotbar(id, Size, CreateSlots(Size, SlotType.Consumable));
        public static Hotbar CreateHotbar() => CreateHotbar(4);

        public static Equipment CreateEquipment(params SlotType[] types) {
            var slots = types.ToDictionary(x => x, x => CreateSlot(x));
            return new Equipment("equipment", slots);
        }

        public static Equipment CreateEquipment() {
            var types = new SlotType[] { SlotType.Weapon1, SlotType.Weapon2, SlotType.Helmet, SlotType.BodyArmor, SlotType.Gloves, SlotType.Boots, SlotType.Ring1, SlotType.Ring2 };
            return CreateEquipment(types);
        }

        public static Chest CreateChest(string id, params Item[] items) {
            var slots = items.ToList().Select(item => CreateSlot(SlotType.RemoveOnly, item));
            return new Chest(id, slots.ToList());

        }

        public static Vendor CreateVendor(string id, params Item[] items) {
            var slots = items.ToList().Select(item => CreateSlot(SlotType.RemoveOnly, item));
            return new Vendor(id, slots.ToList());

        }

        public static Item CreateRandomItem() => CreateItem(GetRandomEnumValue<ItemType>());

        public static Item[] CreateRandomItems(int size) => Enumerable.Range(0, size).Select(_ => CreateItem(GetRandomEnumValue<ItemType>())).ToArray();

        public static Item[] CreateAllItems() {
            var enumItems = new List<ItemType>(Enum.GetValues(typeof(ItemType)) as IEnumerable<ItemType>).Skip(1).ToList();
            return enumItems.Select(item => CreateItem(item)).ToArray();
        }

        private static System.Random random = new System.Random();
        public static T GetRandomEnumValue<T>() where T : Enum {
            var values = new List<T>(Enum.GetValues(typeof(T)) as IEnumerable<T>).Skip(1).ToList();
            int randomIndex = random.Next(values.Count);
            return (T)values[randomIndex];
        }
    }
}