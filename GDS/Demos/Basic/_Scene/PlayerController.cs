using GDS.Basic.Events;
using GDS.Core.Events;
using UnityEngine;

namespace GDS.Basic {

    public class PlayerController : MonoBehaviour {
        public GameObject ConsumeVFX;
        public LayerMask Ground;
        public LayerMask InteractLayer;
        public Transform VFXPos;
        public Material HighlightMaterial;
        Vector3 targetPos;
        RaycastHit hit;
        Transform lastHoveredObject;
        Transform targetObject;

        private void Start() {
            targetPos = transform.position;
        }

        private void OnEnable() {
            EventBus.GlobalBus.Subscribe<ConsumeItemSuccessEvent>(OnConsumeItemSuccess);
        }

        private void OnDisable() {
            EventBus.GlobalBus.Unsubscribe<ConsumeItemSuccessEvent>(OnConsumeItemSuccess);
        }

        void OnConsumeItemSuccess(CustomEvent e) {
            // Create a green particle effect on consume
            Instantiate(ConsumeVFX, VFXPos.position, Quaternion.identity);
        }

        private void OnTriggerEnter(Collider other) {
            // Collide only with Items
            if (other.CompareTag("item")) {
                EventBus.GlobalBus.Publish(new PlayerCollideEvent(other));
            }
        }


        private void Update() {
            if (Store.Instance.IsInventoryOpen.Value == true) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, InteractLayer)) {
                if (hit.collider.transform != lastHoveredObject) {
                    // Highlight object on mouse over
                    if (hit.collider.gameObject.GetComponent<InteractPrefab>()) {
                        lastHoveredObject = hit.collider.transform;
                        lastHoveredObject.GetComponent<Renderer>().material = HighlightMaterial;
                    } else {
                        // Reset material on mouse out
                        if (lastHoveredObject != null) {
                            lastHoveredObject.GetComponent<Renderer>().material = lastHoveredObject.GetComponent<InteractPrefab>().Material;
                            lastHoveredObject = null;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (lastHoveredObject) {
                        // Set target pos in front of the interactible object
                        targetObject = lastHoveredObject;
                        targetPos = lastHoveredObject.position + lastHoveredObject.forward * -0.75f * lastHoveredObject.GetComponent<MeshRenderer>().bounds.size.z;
                    } else {
                        // Set target on the "ground" at mouse pos
                        targetPos = hit.point;
                    }
                    // Look at target pos
                    transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));


                }

                // Move towards target pos
                var newPos = Vector3.MoveTowards(transform.position, targetPos, 0.075f);
                newPos.y = transform.position.y;
                transform.position = newPos;

                // If interacted with something and at target, publish event, then clear the target
                if (targetObject != null && Vector3.Distance(targetPos, newPos) <= 1f) {
                    EventBus.GlobalBus.Publish(new PlayerCollideEvent(targetObject.GetComponent<Collider>()));
                    targetObject = null;
                }

            }

        }


    }
}