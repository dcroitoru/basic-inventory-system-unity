using GDS.Core;

namespace GDS.Basic {
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