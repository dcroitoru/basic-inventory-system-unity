using System.Collections.Generic;
using GDS.Core;

namespace GDS.Basic {
    public record ItemData(Rarity Rarity, int Quant = 1) : Core.ItemData(Quant);
    public record ItemBase(string Id, string Name, string IconPath, bool Stackable, ItemClass Class) : Core.ItemBase(Id, Name, IconPath, Stackable, Sizes.Size1x1);
    public record ArmorItemBase(string Id, string Name, string IconPath, ItemClass Class, int Defense) : ItemBase(Id, Name, IconPath, false, Class);
    public record WeaponItemBase(string Id, string Name, string IconPath, ItemClass Class, int Attack, float AttackSpeed) : ItemBase(Id, Name, IconPath, false, Class);
    public record CraftingOutcomeSlot(Item Item) : Slot(Item);
    public record CraftingBench(string Id, int Size, Observable<List<ListSlot>> Data, Observable<CraftingOutcomeSlot> OutcomeSlot) : ListBag(Id, Size, Data);
    public record Main(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);
    public record Stash(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);
    public record Vendor(string Id, int Size, Observable<List<ListSlot>> Data) : DenseListBag(Id, Size, Data) { public bool Infinite = false; };
    public record Chest(string Id, int Size, Observable<List<ListSlot>> Data) : DenseListBag(Id, Size, Data);
    public record Recipe(Core.ItemBase Item1, Core.ItemBase Item2, Core.ItemBase Item3);
    public record CharacterStats(int Defense, float Damage);


    public enum ItemClass { NoItemClass, Helmet, Gloves, BodyArmor, Boots, Weapon1H, Weapon2H, Consumable, Material, Ring, Amulet, }
    public enum Rarity { NoRarity, Common, Magic, Rare, Unique, }
    public enum SlotType { Helmet, Gloves, BodyArmor, Boots, Weapon, }

}