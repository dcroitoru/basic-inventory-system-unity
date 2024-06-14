// using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

namespace GDS {


    public enum SlotType {
        Default,
        Helmet,
        Gloves,
        BodyArmor,
        Boots,
        Weapon1,
        Weapon2,
        Ring1,
        Ring2,
        Amulet,
        RemoveOnly,
        Consumable
    }

    public enum ItemType {
        NoItem,
        WarriorHelmet,
        SteelGloves,
        LeatherArmor,
        SteelBoots,
        GoldRing,
        Apple,
        HealthPotion,
        ManaPotion,
        Mushroom,
        Sword,
        Silver,
        Wood,
        Gem,
        Dagger,
        Axe,
        Cloak,
        Amulet,
    }

    public enum ItemClass {
        NoClass,
        Helmet,
        Gloves,
        BodyArmor,
        Boots,
        Ring,
        Weapon1H,
        Weapon2H,
        Consumable
    }

    public static class DB {

        public static Dictionary<ItemType, Sprite> Icons => new() {
            { ItemType.WarriorHelmet, Resources.Load<Sprite>("Icons/helmet") },
            { ItemType.SteelGloves, Resources.Load<Sprite>("Icons/gloves") },
            { ItemType.LeatherArmor, Resources.Load<Sprite>("Icons/armor") },
            { ItemType.SteelBoots, Resources.Load<Sprite>("Icons/boots") },
            { ItemType.GoldRing, Resources.Load<Sprite>("Icons/ring") },
            { ItemType.Apple, Resources.Load<Sprite>("Icons/apple") },
            { ItemType.Dagger, Resources.Load<Sprite>("Icons/sword") },
            { ItemType.Sword, Resources.Load<Sprite>("Icons/sword-blue") },
            { ItemType.Axe, Resources.Load<Sprite>("Icons/axe") },
            { ItemType.Cloak, Resources.Load<Sprite>("Icons/cloak") },
            { ItemType.HealthPotion, Resources.Load<Sprite>("Icons/potion") },
            { ItemType.ManaPotion, Resources.Load<Sprite>("Icons/mana") },
            { ItemType.Mushroom, Resources.Load<Sprite>("Icons/mushroom") },
            { ItemType.Silver, Resources.Load<Sprite>("Icons/silver") },
            { ItemType.Wood, Resources.Load<Sprite>("Icons/wood") },
            { ItemType.Gem, Resources.Load<Sprite>("Icons/gem") },
            { ItemType.Amulet, Resources.Load<Sprite>("Icons/necklace") },
        };

        public static Dictionary<ItemType, ItemClass> Classes = new() {
            {ItemType.WarriorHelmet, ItemClass.Helmet},
            {ItemType.SteelGloves, ItemClass.Gloves},
            {ItemType.LeatherArmor, ItemClass.BodyArmor},
            {ItemType.SteelBoots, ItemClass.Boots},
            {ItemType.Sword, ItemClass.Weapon1H},
            {ItemType.Dagger, ItemClass.Weapon1H},
            {ItemType.Axe, ItemClass.Weapon2H},
            {ItemType.Cloak, ItemClass.BodyArmor},
            {ItemType.GoldRing, ItemClass.Ring},
            {ItemType.HealthPotion, ItemClass.Consumable},
            {ItemType.ManaPotion, ItemClass.Consumable},
            {ItemType.Apple, ItemClass.Consumable},
            {ItemType.Mushroom, ItemClass.Consumable}

        };

        public static HashSet<ItemType> Stackable = new() {
            ItemType.Apple,
            ItemType.HealthPotion,
            ItemType.Mushroom,
            ItemType.ManaPotion,
            ItemType.Silver,
            ItemType.Wood,
            ItemType.Gem,
        };


        public static Dictionary<ItemClass, Size> Sizes = new() {
            {ItemClass.Weapon1H, new(1,2)},
            {ItemClass.Weapon2H, new(2,3)},
            {ItemClass.BodyArmor, new(2,3)},
            {ItemClass.Helmet, new(2,2)},
            {ItemClass.Boots, new(2,2)},
            {ItemClass.Gloves, new(2,2)},
        };

        public static Dictionary<SlotType, HashSet<ItemClass>> Accepts = new() {
            {SlotType.Helmet, new() {ItemClass.Helmet}},
            {SlotType.Gloves, new() {ItemClass.Gloves}},
            {SlotType.BodyArmor, new() {ItemClass.BodyArmor}},
            {SlotType.Boots, new() {ItemClass.Boots}},
            {SlotType.Weapon1, new() {ItemClass.Weapon1H, ItemClass.Weapon2H}},
            {SlotType.Weapon2, new() {ItemClass.Weapon1H, ItemClass.Weapon2H}},
            {SlotType.Ring1, new() {ItemClass.Ring}},
            {SlotType.Ring2, new() {ItemClass.Ring}},
            {SlotType.Consumable, new() {ItemClass.Consumable}},
        };
    }
}