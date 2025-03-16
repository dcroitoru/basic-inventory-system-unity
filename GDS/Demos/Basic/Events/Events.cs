using GDS.Core;
using GDS.Core.Events;
using UnityEngine;

namespace GDS.Basic.Events {
    public record PlayerCollideEvent(Collider other) : CustomEvent;
    public record BuyRandomItem(Bag Bag) : CustomEvent;
    public record RerollShop(Bag Bag) : CustomEvent;
    public record ToggleCharacterSheet() : CustomEvent;
}