using UnityEngine;
using GDS.Core.Events;

namespace GDS.Basic {
    [RequireComponent(typeof(AudioSource))]
    public class SfxManager : MonoBehaviour {

        AudioSource audioSource;

        public SoundList Sounds;

        [System.Serializable]
        public class SoundList {
            public AudioClip Fail;
            public AudioClip Pick;
            public AudioClip Place;
            public AudioClip Move;
            public AudioClip Buy;
            public AudioClip Sell;
            public AudioClip Craft;
        }

        void OnEnable() {
            audioSource = GetComponent<AudioSource>();
            var bus = Store.Bus;

            bus.Subscribe<Fail>(PlayClip);
            bus.Subscribe<ActionFail>(PlayClip);
            bus.Subscribe<PickItemSuccess>(PlayClip);
            bus.Subscribe<PlaceItemSuccess>(PlayClip);
            bus.Subscribe<MoveItemSuccess>(PlayClip);
            bus.Subscribe<CraftItemSuccess>(PlayClip);
            bus.Subscribe<BuyItemSuccess>(PlayClip);
            bus.Subscribe<SellItemSuccess>(PlayClip);
            bus.Subscribe<CollectItemSuccess>(PlayClip);
            bus.Subscribe<DropItemSuccess>(PlayClip);
            bus.Subscribe<DestroyItemSuccess>(PlayClip);
        }

        void OnDisable() {
            var bus = Store.Bus;
            bus.Unsubscribe<Fail>(PlayClip);
            bus.Unsubscribe<ActionFail>(PlayClip);
            bus.Unsubscribe<PickItemSuccess>(PlayClip);
            bus.Unsubscribe<PlaceItemSuccess>(PlayClip);
            bus.Unsubscribe<MoveItemSuccess>(PlayClip);
            bus.Unsubscribe<CraftItemSuccess>(PlayClip);
            bus.Unsubscribe<BuyItemSuccess>(PlayClip);
            bus.Unsubscribe<SellItemSuccess>(PlayClip);
            bus.Unsubscribe<CollectItemSuccess>(PlayClip);
            bus.Unsubscribe<DropItemSuccess>(PlayClip);
            bus.Unsubscribe<DestroyItemSuccess>(PlayClip);
        }

        AudioClip EventClip(CustomEvent e) => e switch {
            Fail => Sounds.Fail,
            PickItemSuccess => Sounds.Pick,
            PlaceItemSuccess => Sounds.Place,
            MoveItemSuccess => Sounds.Move,
            CraftItemSuccess => Sounds.Craft,
            BuyItemSuccess => Sounds.Buy,
            SellItemSuccess => Sounds.Sell,
            DropItemSuccess => Sounds.Pick,
            DestroyItemSuccess => Sounds.Place,
            CollectItemSuccess => Sounds.Place,
            _ => null
        };

        void PlayClip(CustomEvent e) {
            audioSource.PlayOneShot(EventClip(e));
        }

    }

}
