using System.Collections.Generic;

namespace GDS.Demos.Basic {
    public static class Data {



        static List<SlotItemDTO> equipment = new() {
            new SlotItemDTO(new(ItemType.WarriorHelmet, 1, ItemRarity.Rare), SlotType.Helmet),
            new SlotItemDTO(new(ItemType.Axe), SlotType.Weapon1)
        };

        static List<ListItemDTO> inventory = new() {
            new ListItemDTO(new(ItemType.LeatherArmor, 1, ItemRarity.Magic), 0),
            new ListItemDTO(new(ItemType.SteelBoots, 1, ItemRarity.Rare), 1),
            new ListItemDTO(new(ItemType.Apple), 2),
            new ListItemDTO(new(ItemType.Apple, 5), 3),
            new ListItemDTO(new(ItemType.Sword, 1, ItemRarity.Unique), 4),
            new ListItemDTO(new(ItemType.Cloak, 1, ItemRarity.Common), 5),
            new ListItemDTO(new(ItemType.SteelGloves, 1, ItemRarity.Magic), 6),
            new ListItemDTO(new(ItemType.Amulet), 7),
            new ListItemDTO(new(ItemType.ManaPotion, 5), 8),
            new ListItemDTO(new(ItemType.Wood, 5), 9),
            new ListItemDTO(new(ItemType.Silver, 5), 10),
            new ListItemDTO(new(ItemType.Dagger), 11),
            new ListItemDTO(new(ItemType.Mushroom, 3), 12),
            new ListItemDTO(new(ItemType.Mushroom, 5), 13),
        };

        static List<ListItemDTO> hotbar = new(){
            new ListItemDTO(new(ItemType.Mushroom, 10), 0),
            new ListItemDTO(new(ItemType.HealthPotion, 15), 1),
            new ListItemDTO(new(ItemType.ManaPotion, 20), 2),
            new ListItemDTO(new(ItemType.Apple, 30), 3),
        };

        static List<ChestDTO> chests = new() {
            new ChestDTO("chest1", new List<ItemDTO>() {
                new ItemDTO(ItemType.Mushroom, 10),
                new ItemDTO(ItemType.Apple, 10),
                new ItemDTO(ItemType.ManaPotion, 10),
                new ItemDTO(ItemType.HealthPotion, 10),
                new ItemDTO(ItemType.GoldRing, 1, ItemRarity.Magic),
                new ItemDTO(ItemType.WarriorHelmet, 1, ItemRarity.Unique),
            })
        };

        public static StateDTO InitialState = new(equipment, inventory, hotbar, chests);
    }
}