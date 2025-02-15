using System;
using UnityEngine;

namespace GDS.Basic {
    public class InteractPrefab : MonoBehaviour {
        [NonSerialized]
        public Material Material;

        private void Awake() {
            Material = GetComponent<Renderer>().material;
        }
    }
}