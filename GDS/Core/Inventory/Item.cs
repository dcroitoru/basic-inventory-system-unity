// This file contains 
// - all types related to Item
// - ItemFactory - a collection of factory methods for creating all types of items
// - ItemExt - a collection of extension methods related to the Item type

namespace GDS.Core {
    public record Item(int Id, ItemBase ItemBase, ItemData ItemData) { public static NoItem NoItem = new(); };
    public record NoItem : Item { public NoItem() : base(-1, ItemFactory.NoItemBase, ItemFactory.NoItemData) { } }
    public record ItemBase(string Id, string Name, string IconPath, bool Stackable, Size Size);
    public record ItemData(int Quant = 1);

    public record Size(int W, int H) {
        public static Size Size1x1 = new(1, 1);
        public static Size Size1x2 = new(1, 2);
        public static Size Size1x3 = new(1, 3);
        public static Size Size1x4 = new(1, 4);
        public static Size Size2x1 = new(2, 1);
        public static Size Size2x2 = new(2, 2);
        public static Size Size2x3 = new(2, 3);
        public static Size Size3x1 = new(3, 1);
        public static Size Size3x2 = new(3, 2);
        public static Size Size3x3 = new(3, 3);
    };

    public static class ItemFactory {
        static int _lastId = 0;
        public static int Id() => _lastId++;
        public static ItemBase NoItemBase = new("NoItem", "NoItem", "", false, Size.Size1x1);
        public static ItemData NoItemData = new();
        public static Item Create(ItemBase itemBase, ItemData itemData) => new(Id(), itemBase, itemData);
        public static Item Create(ItemBase itemBase) => new(Id(), itemBase, NoItemData);
    }

    public static class ItemExt {
        public static string Name(this Item item) => item.ItemBase.Name;
        public static Item Clone(this Item item) => item with { Id = ItemFactory.Id() };
        public static Item Clone(this Item item, ItemData itemData) => item with { Id = ItemFactory.Id(), ItemData = itemData };
        public static T SetQuant<T>(this T item, int quant) where T : Item => item with { ItemData = item.ItemData with { Quant = quant } };
        public static bool CanStack(Item item, Item otherItem) => item.ItemBase.Stackable && item.ItemBase == otherItem.ItemBase;
    }


}