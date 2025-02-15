using UnityEngine;
using UnityEditor;
using GDS.Core;
using UnityEngine.UIElements;

namespace GDS.Minimal {

    public class MainWindow : EditorWindow {

        [MenuItem("Tools/Minimal/Main Window")]
        public static void Open() => GetWindow<MainWindow>().titleContent = new GUIContent("Minimal::Main Window");

        public void CreateGUI() {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("Minimal/MinimalTheme"));
            rootVisualElement.Div(new RootLayer());
        }

    }
}