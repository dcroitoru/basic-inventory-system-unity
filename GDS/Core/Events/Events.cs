using UnityEngine;

// This file contains all the Event types used throughout the system
namespace GDS.Core.Events {
    public abstract record CustomEvent() { public static NoEvent NoEvent = new(); }
    public record NoEvent() : CustomEvent;
    public record ChangedEvent(string Id) : CustomEvent;
    public record AddItemEvent(Item Item) : CustomEvent;
    public record BuyItemEvent(Item Item) : CustomEvent;
    public record PickItemEvent(Bag Bag, Item Item, Slot Slot, EventModifiers EventModifiers) : CustomEvent;
    public record PlaceItemEvent(Bag Bag, Item Item, Slot Slot) : CustomEvent;
    public record DropDraggedItemEvent() : CustomEvent;
    public record DestroyDraggedItemEvent() : CustomEvent;

    public record CollectAllEvent(Bag Bag, Item[] Items) : CustomEvent;
    public record HotbarUseEvent(int indexPlusOne) : CustomEvent;
    public record ToggleInventoryEvent() : CustomEvent;
    public record CloseInventoryEvent() : CustomEvent;
    public record ToggleWindowEvent(string Id) : CustomEvent;
    public record StashTabChangeEvent(int Index) : CustomEvent;

    public record SelectItemEvent(Bag Bag, Item Item) : CustomEvent;
    public record DismantleEvent() : CustomEvent;
    public record CraftEvent(ItemBase ItemBase) : CustomEvent;



    public record ResetEvent() : CustomEvent;
    public record MessageEvent(string Message) : CustomEvent;
    public record PlaySoundEvent() : CustomEvent;
}