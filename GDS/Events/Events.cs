namespace GDS {
    public abstract record CustomEvent();
    public record NoEvent() : CustomEvent;
    public record ChangedEvent(string id) : CustomEvent;
    public record AddItemEvent(Item item) : CustomEvent;
    public record PickItemEvent(Bag Bag, Item Item) : CustomEvent;
    public record PlaceItemEvent(Bag Bag, Item Item, Slot Slot) : CustomEvent;
    public record CollectAllEvent(Bag Bag, Item[] Items) : CustomEvent;
    public record HotbarUseEvent(int indexPlusOne) : CustomEvent;
    public record ToggleInventoryEvent() : CustomEvent;
    public record CloseInventoryEvent() : CustomEvent;
    public record ToggleWindowEvent(string id) : CustomEvent;
    public record SlotHoverEvent(SlotView SlotView) : CustomEvent;


    public record ResetEvent() : CustomEvent;
    public record MessageEvent(string message) : CustomEvent;

}