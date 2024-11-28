using System.Collections.Generic;
using System.Linq;
using GDS.Core;
using UnityEngine;
namespace GDS.Basic {

    public record BasicSetSlot(SlotType Type) : SetSlot(Type.ToString(), Item.NoItem);
    public record BasicItemData(Rarity Rarity, int Quant = 1) : ItemData(Quant);
    public record BasicItemBase(BaseId BaseId, string Name, string IconPath, bool Stackable, ItemClass Class) : Core.ItemBase(BaseId.ToString(), Name, IconPath, Stackable, Size.Size1x1);
    public record ArmorItemBase(BaseId BaseId, string Name, string IconPath, ItemClass Class, int Defense) : BasicItemBase(BaseId, Name, IconPath, false, Class);
    public record WeaponItemBase(BaseId BaseId, string Name, string IconPath, ItemClass Class, int Attack, float AttackSpeed) : BasicItemBase(BaseId, Name, IconPath, false, Class);
    public record Recipe(BaseId one, BaseId two, BaseId three);
    public record CraftingOutcomeSlot(Item Item) : Slot(Item);
    public record CraftingBench(string Id, List<ListSlot> Slots, int Size, CraftingOutcomeSlot OutcomeSlot) : ListBag(Id, Slots, Size);
    public record Vendor(string Id, List<ListSlot> Slots, int Size) : ListBag(Id, Slots, Size);
    public record Stash(string Id, List<ListSlot> Slots, int Size) : ListBag(Id, Slots, Size);
    // TODO: why is this here and not in Core?
    public record InventoryTab(string Name, ListBag Value);

    public static class BasicItemFactory {
        public static Item Create(BaseId baseId, Rarity rarity, int quant = 1) => ItemFactory.Create(InventoryExt.GetBase(baseId), new BasicItemData(rarity, quant));
        public static Item CreateRandom() => ItemFactory.Create(InventoryExt.GetRandomBase(), ItemFactory.NoItemData);
    }

    public static class BasicInventoryFactory {
        public static CraftingBench CreateCraftingBench(string id, int size) {
            var slots = Enumerable.Range(0, size).Select(InventoryFactory.CreateListSlot).ToList();
            var bag = new CraftingBench(id, slots, size, new CraftingOutcomeSlot(Item.NoItem));
            return bag;
        }
    }

    public static class InventoryExt {
        public static BasicItemData ItemData(this Item item) => item.ItemData as BasicItemData;
        public static BasicItemBase ItemBase(this Item item) => item.ItemBase as BasicItemBase;
        public static BaseId BaseId(this Item item) => item.ItemBase is BasicItemBase itembase ? itembase.BaseId : Basic.BaseId.NoBase;
        public static Rarity Rarity(this Item item) => item.ItemData is BasicItemData itemData ? itemData.Rarity : Basic.Rarity.NoRarity;
        public static ItemClass Class(this Item item) => item.ItemBase is BasicItemBase itembase ? itembase.Class : ItemClass.NoItemClass;
        public static BasicItemBase GetRandomBase() => GetBase(EnumUtil.GetRandomEnumValue<BaseId>());
        public static BasicItemBase GetBase(BaseId baseId) {
            var itemBase = DB.AllBasesDict.GetValueOrDefault(baseId);
            if (itemBase != null) return itemBase;
            Debug.LogWarning("Could not find base " + baseId);
            return ItemFactory.NoItemBase as BasicItemBase;
        }
        public static BasicSetSlot GetSlot(this SetBag bag, SlotType slotType) {
            var slot = bag.Slots.GetValueOrDefault(slotType.ToString());
            if (slot != null) return (BasicSetSlot)slot;
            Debug.LogWarning("Could not find slot " + slotType);
            return (BasicSetSlot)InventoryFactory.NoSlot;
        }

        public static bool ConsumeOneOfEachMaterials(this CraftingBench bag) {
            Debug.Log($"should consume one of each items {bag.Slots.CommaJoin()}");
            bag.Consume(0);
            bag.Consume(1);
            bag.Consume(2);
            return true;
        }
    }

    public static class Accepts {
        public static FilterFn Consumable => item => item.Class() == ItemClass.Consumable;
        public static FilterFn Material => item => item.Class() == ItemClass.Material;
        public static FilterFn Weapon => item => item.ItemBase is WeaponItemBase;
        public static FilterFn Armor => item => item.ItemBase is WeaponItemBase;
        public static FilterFn Equipment(SlotType type) => type switch {
            SlotType.Helmet => item => item.Class() == ItemClass.Helmet,
            SlotType.Gloves => item => item.Class() == ItemClass.Gloves,
            SlotType.Boots => item => item.Class() == ItemClass.Boots,
            SlotType.BodyArmor => item => item.Class() == ItemClass.BodyArmor,
            SlotType.Weapon => item => item.ItemBase is WeaponItemBase,
            _ => new((_) => true)
        };
    }
}