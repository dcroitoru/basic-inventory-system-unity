using System;
using UnityEngine;

public class InteractPrefab : MonoBehaviour {
    [NonSerialized]
    public Material Material;

    private void Awake() {
        Material = GetComponent<Renderer>().material;
    }
}
