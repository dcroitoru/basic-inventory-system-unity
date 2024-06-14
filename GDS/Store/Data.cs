using System.Collections.Generic;

namespace GDS {
    public static class Data {
        public static StateDTO InitialState = new(
            new List<SlotItemDTO>() {
                new SlotItemDTO(new(0, ItemType.WarriorHelmet), SlotType.Helmet),
                new SlotItemDTO(new(1, ItemType.Axe), SlotType.Weapon1)
            },
            new List<ListItemDTO>() {
                new ListItemDTO(new(10, ItemType.LeatherArmor), 0),
                new ListItemDTO(new(11, ItemType.SteelBoots), 1),
                new ListItemDTO(new(12, ItemType.Apple), 2),
                new ListItemDTO(new(13, ItemType.Apple, 5), 3),
                new ListItemDTO(new(14, ItemType.Sword), 4),
                new ListItemDTO(new(15, ItemType.Cloak), 5),
                new ListItemDTO(new(16, ItemType.SteelGloves), 6),
                new ListItemDTO(new(17, ItemType.Amulet), 7),
                new ListItemDTO(new(18, ItemType.ManaPotion, 5), 8),
                new ListItemDTO(new(19, ItemType.Wood, 5), 9),
                new ListItemDTO(new(20, ItemType.Silver, 5), 10),
                new ListItemDTO(new(21, ItemType.Dagger, 3), 11),
                new ListItemDTO(new(22, ItemType.Mushroom, 3), 12),
                new ListItemDTO(new(23, ItemType.Mushroom, 5), 13),


            },
            new List<ListItemDTO>(){
                new ListItemDTO(new(50, ItemType.Mushroom, 10), 0),
                new ListItemDTO(new(51, ItemType.HealthPotion, 15), 1),
                new ListItemDTO(new(52, ItemType.ManaPotion, 20), 2),
                new ListItemDTO(new(53, ItemType.Apple, 30), 3),

            },
            new List<ChestDTO>() {
                new ChestDTO("chest1", new List<ItemDTO>() {
                    new ItemDTO(30, ItemType.Mushroom, 10),
                    new ItemDTO(31, ItemType.Apple, 10),
                    new ItemDTO(33, ItemType.ManaPotion, 10),
                    new ItemDTO(34, ItemType.HealthPotion, 10),
                    new ItemDTO(32, ItemType.GoldRing),
                    new ItemDTO(35, ItemType.WarriorHelmet),

                })
             }
        );


        public static SlotType[] BasicEquipmentSlots = new SlotType[] { SlotType.Helmet, SlotType.BodyArmor, SlotType.Boots, SlotType.Weapon1 };
    }
}