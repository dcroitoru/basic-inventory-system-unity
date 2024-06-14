using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDS {

    public class BasicRootWindow : EditorWindow {
        [MenuItem("Tools/Basic Root Window Debug")]
        public static void Open() {
            var wnd = GetWindow<BasicRootWindow>();
            wnd.titleContent = new GUIContent("Basic Root Window Debug");
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            root.styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            root.Div(new BasicRootLayer());

        }

    }
}