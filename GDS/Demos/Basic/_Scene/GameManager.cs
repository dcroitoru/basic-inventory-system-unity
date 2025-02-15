using GDS.Basic.Events;
using GDS.Core;
using GDS.Core.Events;
using UnityEngine;
namespace GDS.Basic {
    public class GameManager : MonoBehaviour {

        public Transform Player;
        public GameObject ItemPrefab;
        public ParticleSystem PickupVFX;

        private void OnEnable() {
            EventBus.GlobalBus.Subscribe<PlayerCollideEvent>(HandleCollisionEvent);
            EventBus.GlobalBus.Subscribe<DropItemSuccessEvent>(HandleInventoryEvent);
        }

        private void OnDisable() {
            EventBus.GlobalBus.Unsubscribe<PlayerCollideEvent>(HandleCollisionEvent);
            EventBus.GlobalBus.Unsubscribe<DropItemSuccessEvent>(HandleInventoryEvent);
        }

        void HandleCollisionEvent(CustomEvent e) {
            if (e is not PlayerCollideEvent ev) return;
            Debug.Log($"Should handle event: {e.ToString().Blue()}");
            var other = ev.other;
            var tag = other.gameObject.tag;
            Debug.Log($"Player collided with a {tag.Orange()}");
            Bag bag = GetBagFor(tag);

            // Check if collided with a "Bag" type object
            if (bag is not NoBag) {
                // Publish an event and return early
                Store.Bus.Publish(new OpenSideWindowEvent(bag));
                return;
            }

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

                // Play a self destroying particle effect then destroy other object
                Instantiate(PickupVFX, other.gameObject.transform.position, Quaternion.identity);
                Destroy(other.gameObject);
                // TODO: add sfx and vfx on item collect perhaps?
            }
        }

        void HandleInventoryEvent(CustomEvent e) {
            Debug.Log("inventory event " + e);
            if (e is not DropItemSuccessEvent ev) return;
            var radius = 3;
            var pos = Player.position + RandomPointOnCircle(radius);
            var instance = Instantiate(ItemPrefab, pos, Quaternion.identity);
            instance.GetComponent<ItemPrefab>().SetItem(ev.Item);
        }

        Vector3 RandomPointOnCircle(int radius) {
            float angle = UnityEngine.Random.Range(0, 360);
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            return new Vector3(x, 0, z);
        }

        Bag GetBagFor(string tag) => tag switch {
            "stash" => Store.Instance.Stash,
            "crafting" => Store.Instance.CraftingBench,
            "shop" => Store.Instance.Vendor,
            "chest" => Store.Instance.Chest,
            _ => Bag.NoBag
        };




    }
}