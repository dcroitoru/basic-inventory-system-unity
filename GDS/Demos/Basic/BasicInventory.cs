using System;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Basic {

    public class BasicInventory : MonoBehaviour {
        [SerializeField]
        UIDocument document;
        EventBus bus = Store.Bus;
        Action<CustomEvent> Publish = Store.Bus.Publish;

        public AudioSource AudioSource;
        public AudioClip[] AudioClips;

        private void Awake() {
        }

        private void OnEnable() {
            document.rootVisualElement.Add(new RootLayer());

            bus.Subscribe<PlaySoundEvent>((e) => playRandomSFX(e as PlaySoundEvent));

        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Publish(new HotbarUseEvent(1));
            if (Input.GetKeyDown(KeyCode.Alpha2)) Publish(new HotbarUseEvent(2));
            if (Input.GetKeyDown(KeyCode.Alpha3)) Publish(new HotbarUseEvent(3));
            if (Input.GetKeyDown(KeyCode.Alpha4)) Publish(new HotbarUseEvent(4));
            if (Input.GetKeyDown(KeyCode.Alpha5)) Publish(new HotbarUseEvent(5));

            if (Input.GetKeyDown(KeyCode.I)) Publish(new ToggleInventoryEvent());
            if (Input.GetKeyDown(KeyCode.Escape)) Publish(new CloseInventoryEvent());
        }

        private void playRandomSFX(PlaySoundEvent e) {
            Debug.Log("Should play some SFX");
        }

    }
}