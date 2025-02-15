using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using GDS.Core;
using static GDS.Core.DebugUtil;

namespace GDS.Minimal {

    public class DebugWindow : EditorWindow {
        [MenuItem("Tools/Minimal/Debug")]
        public static void Open() => GetWindow<DebugWindow>().titleContent = new GUIContent("Minimal::Debug");

        public void CreateGUI() {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("Minimal/MinimalStyles"));
            rootVisualElement.Add("debug-window",
                DraggedItemDebug(Store.Instance.DraggedItem),
                ListBagDebug(Store.Instance.MainInventory)
            );
        }

    }
}