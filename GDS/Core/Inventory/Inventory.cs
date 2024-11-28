using System;
using System.Collections.Generic;

// This file contains all the types related to Inventory (Bag from now on)
namespace GDS.Core {
    public delegate bool FilterFn(Item item);
    public record Pos(int X, int Y);
    public record Slot(Item Item) { public FilterFn Accepts = (_) => true; };

    /// <summary>
    /// A slot with a name, used in inventories like Equipment
    /// </summary>    
    public record SetSlot(string Key, Item Item) : Slot(Item);
    /// <summary>
    /// A slot with an index, used in inventories where slot order matters
    /// </summary>
    public record ListSlot(int Index, Item Item) : Slot(Item);

    /// <summary>
    /// Used in inventories that have named slots (like an Equipment)
    /// </summary>
    public record SetBag(string Id, Dictionary<string, SetSlot> Slots) : Bag(Id);
    /// <summary>
    /// Used in inventories where slots are ordered and item size doesn't matter
    /// </summary>    
    public record ListBag(string Id, List<ListSlot> Slots, int Size) : Bag(Id);


    public record NoBag() : Bag("NoBag");
    /// <summary>
    /// Base type for all bags (inventories)
    /// </summary>
    /// <param name="Id"></param>
    public abstract record Bag(string Id) : INotifyChange {
        public static NoBag NoBag = new();
        public event Action<object> OnChange = (_) => { };
        public void Notify() => OnChange(this);
        public FilterFn Accepts = Core.Accepts.Everything;
    }

    /// <summary>
    /// Contains some filtering functions used to control item restriction in slots and inventories
    /// </summary>
    public static class Accepts {
        public static FilterFn Everything = (_) => true;
        public static FilterFn Nothing = (_) => false;
    }
}