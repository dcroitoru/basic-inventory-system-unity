using GDS.Core.Events;
using UnityEngine;

namespace GDS.Basic.Events {
    public record PlayerCollideEvent(Collider other) : CustomEvent;
}