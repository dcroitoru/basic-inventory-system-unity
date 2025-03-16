using System;
using UnityEngine;
using GDS.Core.Events;
using GDS.Basic.Events;

namespace GDS.Basic {
    public class InputManager : MonoBehaviour {

        Action<CustomEvent> Publish = Store.Bus.Publish;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Publish(new HotbarUseEvent(1));
            if (Input.GetKeyDown(KeyCode.Alpha2)) Publish(new HotbarUseEvent(2));
            if (Input.GetKeyDown(KeyCode.Alpha3)) Publish(new HotbarUseEvent(3));
            if (Input.GetKeyDown(KeyCode.Alpha4)) Publish(new HotbarUseEvent(4));
            if (Input.GetKeyDown(KeyCode.Alpha5)) Publish(new HotbarUseEvent(5));

            if (Input.GetKeyDown(KeyCode.C)) Publish(new ToggleCharacterSheet());
            if (Input.GetKeyDown(KeyCode.I)) Publish(new ToggleInventoryEvent());
            if (Input.GetKeyDown(KeyCode.Tab)) Publish(new ToggleInventoryEvent());
            if (Input.GetKeyDown(KeyCode.Escape)) Publish(new CloseInventoryEvent());
        }
    }
}