using System;
using System.Collections.Generic;
using System.Linq;
using GDS.Core;
using static GDS.Basic.BaseId;
using static GDS.Basic.SlotType;
namespace GDS.Basic {

    public static class DB {
        public static List<BasicItemBase> AllBases = new() {
            new ArmorItemBase(WarriorHelmet, "Warrior Helmet", "Shared/Icons/helmet", ItemClass.Helmet, 40),
            new ArmorItemBase(SteelBoots, "Steel Boots", "Shared/Icons/boots", ItemClass.Boots, 30),
            new ArmorItemBase(LeatherArmor, "Leather Armor", "Shared/Icons/armor", ItemClass.BodyArmor, 120),
            new ArmorItemBase(LeatherGloves, "Leather Gloves", "Shared/Icons/gloves", ItemClass.Gloves, 25),
            new WeaponItemBase(LongSword, "Long Sword", "Shared/Icons/sword", ItemClass.Weapon1H, 110, 1.35f),
            new WeaponItemBase(ShortSword, "Short Sword", "Shared/Icons/sword-blue", ItemClass.Weapon1H, 80, 1.5f),
            new WeaponItemBase(Axe, "Axe", "Shared/Icons/axe",  ItemClass.Weapon2H, 140, 1.2f),
            new (GoldRing, "Gold Ring", "Shared/Icons/ring", false,  ItemClass.Ring),
            new (BlueAmulet, "Silver Amulet", "Shared/Icons/necklace", false,  ItemClass.Amulet),
            new (Apple, "Apple", "Shared/Icons/apple", true,  ItemClass.Consumable),
            new (Potion, "Potion", "Shared/Icons/potion", true,  ItemClass.Consumable),
            new (Mushroom, "Mushroom", "Shared/Icons/mushroom", true,  ItemClass.Consumable),
            new (Wood, "Wood", "Shared/Icons/wood", true, ItemClass.Material),
            new (Steel, "Steel", "Shared/Icons/silver", true, ItemClass.Material),
            new (Gem, "Gem", "Shared/Icons/gem", true, ItemClass.Material),
        };

        public static Dictionary<BaseId, BasicItemBase> AllBasesDict = AllBases.ToDictionary(x => x.BaseId);

        public static Dictionary<Recipe, BaseId> Recipes = new() {
            {new (Wood, Wood, Steel), Axe},
            {new (Wood, Steel, Steel), LongSword},
            {new (Gem, Wood, Steel), ShortSword},
            {new (Gem, Steel, NoBase), BlueAmulet},
        };

        public static BasicSetSlot[] EquipmentSlots = Array.ConvertAll<SlotType, BasicSetSlot>(
            EnumUtil.GetAllEnumValues<SlotType>(),
            value => new BasicSetSlot(value) { Accepts = Accepts.Equipment(value) }
        );
    }

    public enum BaseId {
        NoBase,
        WarriorHelmet,
        LeatherArmor,
        SteelBoots,
        LeatherGloves,
        LongSword,
        ShortSword,
        GoldRing,
        BlueAmulet,
        Axe,
        Apple,
        Potion,
        Mushroom,
        Wood,
        Steel,
        Gem

    }

    public enum SlotType {
        Helmet,
        Gloves,
        BodyArmor,
        Boots,
        Weapon,
    }

    public enum ItemClass {
        NoItemClass,
        Helmet,
        Gloves,
        BodyArmor,
        Boots,
        Weapon1H,
        Weapon2H,
        Consumable,
        Material,
        Ring,
        Amulet,

    }

    public enum Rarity {
        NoRarity,
        Common,
        Magic,
        Rare,
        Unique,
    }




}