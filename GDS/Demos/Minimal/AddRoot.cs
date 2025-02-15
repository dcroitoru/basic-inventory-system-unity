using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Minimal {

    public class AddRoot : MonoBehaviour {
        [SerializeField]
        UIDocument document;

        // Add the `RootLayer` element to the document 
        public void OnEnable() => document.rootVisualElement.Add(new RootLayer());

    }
}