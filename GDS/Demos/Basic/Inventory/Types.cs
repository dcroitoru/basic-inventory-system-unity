using System.Collections.Generic;
using System.Linq;
using GDS.Core;

namespace GDS.Basic {
    public record ItemData(Rarity Rarity, int Quant = 1) : Core.ItemData(Quant);
    public record ItemBase(string Id, string Name, string IconPath, bool Stackable, ItemClass Class) : Core.ItemBase(Id, Name, IconPath, Stackable, Sizes.Size1x1);
    public record ArmorItemBase(string Id, string Name, string IconPath, ItemClass Class, int Defense) : ItemBase(Id, Name, IconPath, false, Class);
    public record WeaponItemBase(string Id, string Name, string IconPath, ItemClass Class, int Attack, float AttackSpeed) : ItemBase(Id, Name, IconPath, false, Class);
    public record CraftingOutcomeSlot(Item Item) : Slot(Item);
    public record CraftingBench(string Id, int Size, Observable<List<ListSlot>> Data, Observable<CraftingOutcomeSlot> OutcomeSlot) : ListBag(Id, Size, Data);
    public record Main(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);
    public record Vendor(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);
    public record Stash(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);
    public record Recipe(Core.ItemBase one, Core.ItemBase two, Core.ItemBase three);

    public enum ItemClass { NoItemClass, Helmet, Gloves, BodyArmor, Boots, Weapon1H, Weapon2H, Consumable, Material, Ring, Amulet, }
    public enum Rarity { NoRarity, Common, Magic, Rare, Unique, }
    public enum SlotType { Helmet, Gloves, BodyArmor, Boots, Weapon, }

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

        public static readonly IEnumerable<ItemBase> All = typeof(Bases).GetFields().Select(field => (ItemBase)field.GetValue(null));

        public static ItemBase RandomBase() {
            var rnd = new System.Random();
            var randomIndex = rnd.Next(All.Count() - 1);
            var randomBase = All.ElementAt(randomIndex);
            return randomBase;
        }


    }

    // Note: 
    // Here we are using `null` instead of a `Core.NoItemBase` because `Core.NoItemBase`
    // cannot be auto upcasted to `Basic.ItemBase` and defaults to `null`
    // This will change when the type system will be further improved (not sure how yet)
    public static class Recipes {
        public static readonly Dictionary<Recipe, ItemBase> All = new() {
            {new (Bases.Wood, Bases.Wood, Bases.Steel), Bases.Axe},
            {new (Bases.Wood, Bases.Steel, Bases.Steel), Bases.LongSword},
            {new (Bases.Gem, Bases.Wood, Bases.Steel), Bases.ShortSword},
            {new (Bases.Gem, Bases.Steel, null), Bases.BlueAmulet},
        };

    }

    public static class Filters {
        public static readonly FilterFn Weapon = item => item.ItemBase is WeaponItemBase;
        public static readonly FilterFn Armor = item => item.ItemBase is WeaponItemBase;
        public static readonly FilterFn Consumable = item => item.Class() == ItemClass.Consumable;
        public static readonly FilterFn Material = item => item.Class() == ItemClass.Material;

        public static readonly FilterFn Helmet = item => item.Class() == ItemClass.Helmet;
        public static readonly FilterFn Gloves = item => item.Class() == ItemClass.Gloves;
        public static readonly FilterFn Boots = item => item.Class() == ItemClass.Boots;
        public static readonly FilterFn BodyArmor = item => item.Class() == ItemClass.BodyArmor;
    }

}