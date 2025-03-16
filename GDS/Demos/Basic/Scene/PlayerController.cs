using UnityEngine;
using GDS.Basic.Events;
using GDS.Core.Events;
using UnityEngine.UIElements;
using System.Linq;


namespace GDS.Basic {
    public class PlayerController : MonoBehaviour {
        [SerializeField] GameObject ConsumeVFX;
        [SerializeField] Material HighlightMaterial;
        [SerializeField] Transform VFXPos;
        [SerializeField] UIDocument uiDocument;
        Vector3 targetPos;
        RaycastHit hit;
        InteractPrefab lastHoveredObject;
        InteractPrefab targetObject;
        IPanel panel;
        VisualElement root;
        Vector2 screenPos;
        VisualElement picked;
        Ray ray;
        float arw, arh;

        private void Start() {
            targetPos = transform.position;
            root = uiDocument.rootVisualElement.Children().First();
            panel = uiDocument.rootVisualElement.panel;

        }

        private void OnEnable() {
            Store.Bus.Subscribe<ConsumeItemSuccess>(OnConsumeItemSuccess);
        }

        private void OnDisable() {
            Store.Bus.Unsubscribe<ConsumeItemSuccess>(OnConsumeItemSuccess);
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
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100)) {
                if (hit.collider.transform != lastHoveredObject) {
                    // Highlight object on mouse over
                    if (hit.collider.gameObject.GetComponent<InteractPrefab>()) {
                        if (lastHoveredObject) lastHoveredObject.Reset();
                        lastHoveredObject = hit.collider.gameObject.GetComponent<InteractPrefab>();
                        lastHoveredObject.Highlight(HighlightMaterial);
                    } else {
                        // Reset material on mouse out
                        if (lastHoveredObject != null) {
                            lastHoveredObject.Reset();
                            lastHoveredObject = null;
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0)) {
                    // Convert screen position to panel position
                    ScreenToPanel(Input.mousePosition, out screenPos);
                    // Pick element at position
                    picked = panel.Pick(screenPos);
                    // If the element is anything other than the root, it means we are interacting with UI and should return early to avoid
                    // player movement and interacting with scene objects
                    if (picked != root) return;
                    if (lastHoveredObject) {
                        // Set target pos in front of the interactible object
                        targetObject = lastHoveredObject;
                        targetPos = lastHoveredObject.transform.position + lastHoveredObject.transform.forward * -1 * lastHoveredObject.GetComponent<MeshRenderer>().bounds.size.z;
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

        void ScreenToPanel(Vector2 pos, out Vector2 screenPos) {
            arw = uiDocument.rootVisualElement.worldBound.width / Screen.width;
            arh = uiDocument.rootVisualElement.worldBound.height / Screen.height;
            screenPos.x = pos.x * arw;
            screenPos.y = (Screen.height - pos.y) * arh;
        }
    }
}