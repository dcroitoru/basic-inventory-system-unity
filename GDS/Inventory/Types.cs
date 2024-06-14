using System.Collections.Generic;

namespace GDS {

    public record ItemId(int Value);
    public record SlotId(int Value);
    public record Size(int W, int H);
    public record Pos(int X, int Y);

    public record Item(ItemId Id, ItemType Type, int Quant, Size Size, ItemClass Class);
    public record Slot(SlotId Id, SlotType Type, Item Item);

    public abstract record Bag(string Id);
    public record NoBag(string id) : Bag(id);
    public record Equipment(string Id, Dictionary<SlotType, Slot> Slots) : Bag(Id);
    public record Inventory(string Id, int Size, List<Slot> Slots) : Bag(Id);
    public record Hotbar(string Id, int Size, List<Slot> Slots) : Inventory(Id, Size, Slots);
    public record Chest(string Id, List<Slot> Slots) : Inventory(Id, 0, Slots);
    public record Vendor(string Id, List<Slot> Slots) : Inventory(Id, 0, Slots);

    public record ItemDTO(int id, ItemType type, int quant = 1);
    public record SlotItemDTO(ItemDTO item, SlotType pos);
    public record ListItemDTO(ItemDTO item, int pos);
    public record ChestDTO(string id, List<ItemDTO> items);
    public record StateDTO(
        List<SlotItemDTO> equipment,
        List<ListItemDTO> inventory,
        List<ListItemDTO> hotbar,
        List<ChestDTO> chests
    );

}

