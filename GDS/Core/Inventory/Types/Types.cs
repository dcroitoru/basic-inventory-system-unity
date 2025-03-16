using System;
using System.Collections.Generic;

namespace GDS.Core {

    public record ItemBase(string Id, string Name, string IconPath, bool Stackable, Size Size) { public static ItemBase NoItemBase = new("NoItem", "NoItem", "NoPath", false, Sizes.Size1x1); };
    public record ItemData(int Quant = 1) { public static ItemData NoItemData = new(); };
    public record Item(int Id, ItemBase ItemBase, ItemData ItemData) { public static readonly NoItem NoItem = new(); };
    public record NoItem() : Item(-1, ItemBase.NoItemBase, ItemData.NoItemData);
    public record BagItem(Bag Bag, Item Item);

    public record Pos(int X, int Y) { public static readonly NoPos NoPos = new(); };
    public record NoPos() : Pos(0, 0);
    public record Size(int W, int H) { public static readonly NoSize NoSize = new(); };
    public record NoSize() : Size(0, 0);

    public delegate bool Filter(Item item);
    public delegate bool FilterFn(Item item);

    public record Slot(Item Item) { public static Slot NoSlot = new(Item.NoItem); public FilterFn Accepts = Filters.Everything; };

    /// <summary>
    /// A slot with a name, used in inventories like Equipment
    /// </summary>    
    public record SetSlot(string Key, Item Item) : Slot(Item);
    /// <summary>
    /// A slot with an index, used in inventories where slot order matters
    /// </summary>
    public record ListSlot(int Index, Item Item) : Slot(Item);

    /// <summary>
    /// Base type for all bags (inventories)
    /// </summary>
    /// <param name="Id"></param>
    public abstract record Bag(string Id) {
        public static NoBag NoBag = new();
        public FilterFn Accepts = Filters.Everything;
    }
    public record NoBag() : Bag("NoBag");

    /// <summary>
    /// Used in inventories that have named slots
    /// For example an Equipment Bag could have 2 weapon slots differentated by their slot name (Main-Hand, Off-Hand)
    /// </summary>
    public record SetBag(string Id, Observable<Dictionary<string, SetSlot>> Data) : Bag(Id) {
        public Dictionary<string, SetSlot> Slots = Data.Value;
    };
    /// <summary>
    /// Used in inventories where slots are ordered and item size doesn't matter
    /// </summary>    
    public record ListBag(string Id, int Size, Observable<List<ListSlot>> Data) : Bag(Id) {
        public List<ListSlot> Slots = Data.Value;
    };

    /// <summary>
    /// Used in inventories where slots are ordered and item size doesn't matter
    /// </summary>    
    public record DenseListBag(string Id, int Size, Observable<List<ListSlot>> Data) : ListBag(Id, Size, Data);

}