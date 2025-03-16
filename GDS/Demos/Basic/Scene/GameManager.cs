using UnityEngine;
using GDS.Basic.Events;
using GDS.Core;
using GDS.Core.Events;
using UnityEditor;
namespace GDS.Basic {
    public class GameManager : MonoBehaviour {

        public Transform Player;
        public GameObject ItemPrefab;
        public Material ItemMaterial;
        public ParticleSystem PickupVFX;

        private void OnEnable() {
            EventBus.GlobalBus.Subscribe<PlayerCollideEvent>(HandleCollisionEvent);
            Store.Bus.Subscribe<DropItemSuccess>(HandleInventoryEvent);
            Store.Bus.Subscribe<ResetEvent>(OnReset);
#if UNITY_EDITOR
            Store.ColorSpace.SetValue(PlayerSettings.colorSpace);
#endif
        }

        private void OnDisable() {
            EventBus.GlobalBus.Unsubscribe<PlayerCollideEvent>(HandleCollisionEvent);
            Store.Bus.Unsubscribe<DropItemSuccess>(HandleInventoryEvent);
            Store.Bus.Unsubscribe<ResetEvent>(OnReset);
        }

        void OnReset(CustomEvent e) {
            var objects = GameObject.FindGameObjectsWithTag("item");
            foreach (var item in objects) { Destroy(item); }
        }

        void HandleCollisionEvent(CustomEvent e) {
            if (e is not PlayerCollideEvent ev) return;
            var other = ev.other;
            var bagId = other.GetComponentInParent<BagId>()?.Id;
            if (bagId != null) Store.Bus.Publish(new OpenSideWindowByIdEvent(bagId));

            var bag = other.GetComponentInParent<BagData>()?.Bag;
            if (bag != null) Store.Bus.Publish(new OpenSideWindowEvent(bag));


            // Check if collided with an `Item`
            if (other.gameObject.GetComponent<ItemPrefab>() != null) {
                // Check if inventory full
                if (Store.Instance.Main.IsFull()) {
                    // Publish an event and return early
                    EventBus.GlobalBus.Publish(new MessageEvent("Inventory full"));
                    return;
                }

                // Get `Item` from component and publish en event
                var item = other.gameObject.GetComponent<ItemPrefab>().Item;
                Store.Bus.Publish(new AddItemEvent(item));

                // Play a particle effect then destroy other object
                Instantiate(PickupVFX, other.gameObject.transform.position, Quaternion.identity);
                Destroy(other.gameObject);
            }
        }

        void HandleInventoryEvent(CustomEvent e) {
            Debug.Log("inventory event " + e);
            if (e is not DropItemSuccess ev) return;
            var radius = 3;
            var pos = Player.position + RandomPointOnCircle(radius);
            var instance = Instantiate(ItemPrefab, pos, Quaternion.identity);
            instance.GetComponent<ItemPrefab>().SetItem(ev.Item);
            instance.GetComponent<Renderer>().material = ItemMaterial;
        }

        Vector3 RandomPointOnCircle(int radius) {
            float angle = UnityEngine.Random.Range(0, 360);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            return new Vector3(x, 0, z);
        }

    }
}