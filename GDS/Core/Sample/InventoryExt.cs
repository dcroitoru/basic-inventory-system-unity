using System.Collections.Generic;
using GDS.Core;
using UnityEngine;
namespace GDS.Sample {
    /// <summary>
    /// Adds an `ItemClass` field to `ItemBase`
    /// </summary>
    public record SampleItemBase(BaseId BaseId, string Name, string IconPath, bool Stackable, Size Size, ItemClass Class) : Core.ItemBase(BaseId.ToString(), Name, IconPath, Stackable, Size);
    /// <summary>
    /// Adds a `Rarity` field to `ItemData`
    /// </summary>
    public record SampleItemData(Rarity Rarity, int Quant = 1) : ItemData(Quant);
    /// <summary>
    /// Adds `SlotType` field to `SetSlot`
    /// </summary>
    public record SampleSetSlot(SlotType Type) : SetSlot(Type.ToString(), Item.NoItem);

    /// <summary>
    /// Item factory
    /// </summary>
    public static class ItemFactory {
        public static Item Create(BaseId baseId, Rarity rarity, int quant = 1)
            => Core.ItemFactory.Create(InventoryExt.GetBase(baseId), new SampleItemData(rarity, quant));
        public static Item CreateRandom() => Core.ItemFactory.Create(InventoryExt.GetRandomBase(), Core.ItemFactory.NoItemData);
    }

    /// <summary>
    /// Inventory Extension Methods
    /// </summary>
    public static class InventoryExt {
        public static SampleItemData ItemData(this Item item) => item.ItemData as SampleItemData;
        public static SampleItemBase ItemBase(this Item item) => item.ItemBase as SampleItemBase;
        public static SampleItemBase GetRandomBase() => GetBase(EnumUtil.GetRandomEnumValue<BaseId>());
        public static Rarity Rarity(this Item item) => item.ItemData is SampleItemData i ? i.Rarity : Sample.Rarity.NoRarity;
        public static ItemClass Class(this Item item) => item.ItemBase is SampleItemBase b ? b.Class : Sample.ItemClass.NoItemClass;
        public static SampleItemBase GetBase(BaseId baseId) {
            var itemBase = DB.AllBasesDict.GetValueOrDefault(baseId);
            if (itemBase != null) return itemBase;
            Debug.LogWarning("Could not find base " + baseId);
            return Core.ItemFactory.NoItemBase as SampleItemBase;
        }

        public static SampleSetSlot GetSlot(this SetBag bag, SlotType slotType) {
            var slot = bag.Slots.GetValueOrDefault(slotType.ToString());
            if (slot != null) return (SampleSetSlot)slot;
            Debug.LogWarning("Could not find slot " + slotType);
            return (SampleSetSlot)InventoryFactory.NoSlot;
        }
    }

    /// <summary>
    /// Filter functions used to check if a slot or inventory accepts an item
    /// </summary>
    public static class Accepts {
        public static FilterFn Equipment(SlotType type) => type switch {
            SlotType.Helmet => item => item.Class() == ItemClass.Helmet,
            SlotType.Gloves => item => item.Class() == ItemClass.Gloves,
            SlotType.Weapon => item => item.Class() == ItemClass.Weapon1H,
            SlotType.Boots => item => item.Class() == ItemClass.Boots,
            SlotType.BodyArmor => item => item.Class() == ItemClass.BodyArmor,
            _ => new((_) => true)
        };
    }
}