using System.Collections.Generic;
using System.Linq;

namespace GDS.Basic {
    public static class Bases {
        public static readonly ArmorItemBase WarriorHelmet = new("WarriorHelmet", "Warrior Helmet", "Shared/Images/items/helmet", ItemClass.Helmet, 40);
        public static readonly ArmorItemBase LeatherGloves = new("LeatherGloves", "Leather Gloves", "Shared/Images/items/gloves", ItemClass.Gloves, 25);
        public static readonly ArmorItemBase LeatherArmor = new("LeatherArmor", "Leather Armor", "Shared/Images/items/armor", ItemClass.BodyArmor, 120);
        public static readonly ArmorItemBase SteelBoots = new("SteelBoots", "Steel Boots", "Shared/Images/items/boots", ItemClass.Boots, 30);

        public static readonly WeaponItemBase ShortSword = new("ShortSword", "Short Sword", "Shared/Images/items/sword-blue", ItemClass.Weapon1H, 80, 1.5f);
        public static readonly WeaponItemBase LongSword = new("LongSword", "Long Sword", "Shared/Images/items/sword", ItemClass.Weapon1H, 110, 1.35f);
        public static readonly WeaponItemBase Axe = new("Axe", "Axe", "Shared/Images/items/axe", ItemClass.Weapon2H, 140, 1.2f);

        public static readonly ItemBase BlueAmulet = new("BlueAmulet", "Silver Amulet", "Shared/Images/items/necklace", false, ItemClass.Amulet);
        public static readonly ItemBase Mushroom = new("Mushroom", "Mushroom", "Shared/Images/items/mushroom", true, ItemClass.Consumable);
        public static readonly ItemBase GoldRing = new("GoldRing", "Gold Ring", "Shared/Images/items/ring", false, ItemClass.Ring);
        public static readonly ItemBase Potion = new("Potion", "Potion", "Shared/Images/items/potion", true, ItemClass.Consumable);
        public static readonly ItemBase Apple = new("Apple", "Apple", "Shared/Images/items/apple", true, ItemClass.Consumable);
        public static readonly ItemBase Steel = new("Steel", "Steel", "Shared/Images/items/silver", true, ItemClass.Material);
        public static readonly ItemBase Wood = new("Wood", "Wood", "Shared/Images/items/wood", true, ItemClass.Material);
        public static readonly ItemBase Gem = new("Gem", "Gem", "Shared/Images/items/gem", true, ItemClass.Material);

        // public static readonly IEnumerable<ItemBase> All = typeof(Bases).GetFields().Select(field => (ItemBase)field.GetValue(null));
        public static readonly IEnumerable<ItemBase> All = typeof(Bases).GetFields().Where(field => typeof(ItemBase).IsAssignableFrom(field.FieldType)).Select(field => (ItemBase)field.GetValue(null));

        public static ItemBase RandomBase() {
            var rnd = new System.Random();
            var randomIndex = rnd.Next(All.Count() - 1);
            var randomBase = All.ElementAt(randomIndex);
            return randomBase;
        }

        public static ItemBase Get(string id) => All.Where(b => b.Id == id).FirstOrDefault();
        public static readonly string[] AllIds = All.Select(b => b.Id).ToArray();


    }
}