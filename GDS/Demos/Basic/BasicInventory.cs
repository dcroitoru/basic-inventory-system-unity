using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS {

    public class BasicInventory : MonoBehaviour {
        [SerializeField] UIDocument document;
        EventBus bus = Global.BasicBus;
        private void Awake() {
            var root = document.rootVisualElement;
            root.Div(new BasicRootLayer());
        }



        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) bus.Publish(new HotbarUseEvent(1));
            if (Input.GetKeyDown(KeyCode.Alpha2)) bus.Publish(new HotbarUseEvent(2));
            if (Input.GetKeyDown(KeyCode.Alpha3)) bus.Publish(new HotbarUseEvent(3));
            if (Input.GetKeyDown(KeyCode.Alpha4)) bus.Publish(new HotbarUseEvent(4));
            if (Input.GetKeyDown(KeyCode.Alpha5)) bus.Publish(new HotbarUseEvent(5));

            if (Input.GetKeyDown(KeyCode.I)) bus.Publish(new ToggleInventoryEvent());
            if (Input.GetKeyDown(KeyCode.Escape)) bus.Publish(new CloseInventoryEvent());
        }

    }
}