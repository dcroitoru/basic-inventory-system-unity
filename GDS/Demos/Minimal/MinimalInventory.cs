using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Minimal {

    public class Inventory : MonoBehaviour {
        [SerializeField]
        UIDocument document;

        // Add the `RootLayer` to the document 
        public void OnEnable() => document.rootVisualElement.Add(new RootLayer());

    }
}