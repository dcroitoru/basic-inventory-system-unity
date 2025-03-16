using System;
using UnityEngine;

namespace GDS.Basic {
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Renderer))]
    public class InteractPrefab : MonoBehaviour {
        [NonSerialized] public Material InitialMaterial;
        [SerializeField] public Collider Collider;
        [SerializeField] Renderer Renderer;

        private void Awake() {
            InitialMaterial = GetComponent<Renderer>().material;
            Collider = GetComponent<Collider>();
            Renderer = GetComponent<Renderer>();
        }

        public void Highlight(Material material) {
            Renderer.material = material;
        }

        public void Reset() {
            Renderer.material = InitialMaterial;
        }

    }
}