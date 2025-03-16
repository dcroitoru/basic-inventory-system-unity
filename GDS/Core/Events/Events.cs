using UnityEngine;


// This file contains all the Event types used throughout the system
namespace GDS.Core.Events {

    public abstract record Error;
    public record NoError : Error;
    public record BagFull : Error;

    public abstract record CustomEvent() { public static NoEvent NoEvent = new(); public static Fail Fail = new(); public static Success Success = new(); }
    public record NoEvent() : CustomEvent;
    public record ChangedEvent(string Id) : CustomEvent;
    public record AddItemEvent(Item Item) : CustomEvent;
    public record BuyItemEvent(Item Item) : CustomEvent;
    public record SellItemEvent(Item Item) : CustomEvent;
    public record PickItemEvent(Bag Bag, Item Item, Slot Slot, EventModifiers EventModifiers) : CustomEvent;
    public record PlaceItemEvent(Bag Bag, Item Item, Slot Slot, EventModifiers EventModifiers) : CustomEvent;
    public record DropDraggedItemEvent() : CustomEvent;
    public record DestroyDraggedItemEvent() : CustomEvent;


    public record CollectAllEvent(Bag Bag, Item[] Items) : CustomEvent;
    public record HotbarUseEvent(int indexPlusOne) : CustomEvent;
    public record ToggleInventoryEvent() : CustomEvent;
    public record CloseInventoryEvent() : CustomEvent;
    public record ToggleWindowEvent(string Id) : CustomEvent;
    public record StashTabChangeEvent(int Index) : CustomEvent;
    public record OpenSideWindowEvent(Bag Bag) : CustomEvent;
    public record CloseWindowEvent(Bag Bag) : CustomEvent;
    public record OpenSideWindowByIdEvent(string Id) : CustomEvent;

    public record SelectItemEvent(Bag Bag, Item Item) : CustomEvent;
    public record DismantleEvent() : CustomEvent;
    public record CraftEvent(ItemBase ItemBase) : CustomEvent;

    public record ResetEvent() : CustomEvent;
    public record MessageEvent(string Message) : CustomEvent;
    public record PlaySoundEvent() : CustomEvent;

    public record RotateItemEvent() : CustomEvent;
    public record IllegalActionEvent() : CustomEvent;


    public record Success : CustomEvent;

    public record PickItemSuccess(Item Item) : Success;
    public record PlaceItemSuccess(Item Item) : Success;
    public record MoveItemSuccess(Item Item) : Success;
    public record BuyItemSuccess(Item Item) : Success;
    public record SellItemSuccess(Item Item) : Success;
    public record DropItemSuccess(Item Item) : Success;
    public record ConsumeItemSuccess(Item Item) : Success;
    public record CraftItemSuccess(Item Item) : Success;
    public record CollectItemSuccess(Item Item) : Success;
    public record DestroyItemSuccess(Item Item) : Success;

    public enum Severity { Info, Warning, Error }
    public record Fail() : CustomEvent;
    public record ActionFail(string Reason, Severity Severity = Severity.Info) : Fail;
}