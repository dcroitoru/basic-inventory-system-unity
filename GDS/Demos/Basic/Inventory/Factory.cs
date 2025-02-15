using System.Linq;
using GDS.Core;
using GDS.Core.Events;
using static GDS.Core.InventoryFactory;

namespace GDS.Basic {
    public static class Factory {
        public static Item CreateItem(ItemBase itemBase, Rarity rarity, int quant = 1) => InventoryFactory.Create(itemBase, new ItemData(rarity, quant));
        public static Item CreateRandom() => InventoryFactory.Create(Bases.RandomBase(), Core.ItemData.NoItemData);

        public static CraftingBench CreateCraftingBench(string id, int size) {
            var slots = Enumerable.Range(0, size).Select(InventoryFactory.CreateListSlot).ToList();
            var bag = new CraftingBench(id, size, new(slots), new(new(Item.NoItem)));
            return bag;
        }

        public static SetSlot[] CreateEquipmentSlots() => new SetSlot[] {
            CreateSetSlot(SlotType.Helmet.ToString(), Filters.Helmet),
            CreateSetSlot(SlotType.BodyArmor.ToString(), Filters.BodyArmor),
            CreateSetSlot(SlotType.Gloves.ToString(), Filters.Gloves),
            CreateSetSlot(SlotType.Boots.ToString(), Filters.Boots),
            CreateSetSlot(SlotType.Weapon.ToString(), Filters.Weapon),
        };

        public static MessageEvent CreateMessageEvent(string action, Item item, bool showQuant = true) {
            var quantText = (showQuant && item.ItemBase.Stackable) ? $" ({item.ItemData.Quant})" : "";
            return new MessageEvent(action + " " + $"[{item.Name()}{quantText}]".Blue());
        }


    }
}