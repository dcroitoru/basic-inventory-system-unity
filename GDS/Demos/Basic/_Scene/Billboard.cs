using UnityEngine;

namespace GDS.Basic {
    public class Billboard : MonoBehaviour {
        void LateUpdate() {
            // Make the label face the camera
            transform.forward = Camera.main.transform.forward;
        }
    }
}