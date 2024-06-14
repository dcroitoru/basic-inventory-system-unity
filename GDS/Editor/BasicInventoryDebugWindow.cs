using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using GDS;
using static GDS.Dom;
namespace GDS {

    public class BasicInventoryDebugWindow : EditorWindow {
        [MenuItem("Tools/Basic Inventory Debug")]
        public static void Open() {
            var wnd = GetWindow<BasicInventoryDebugWindow>();
            wnd.titleContent = new GUIContent("Basic Inventory Debug");
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            root.styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            root.
                Div(
                    new BasicInventoryWindow(),
                    Button("", "Add Random Item", () => OnAddClicked(ItemType.Dagger)),
                    Button("", "Reset", () => OnResetClicked()),
                    new BasicGhostItemView(root, Global.BasicBus)

            );


        }

        void OnAddClicked(ItemType type) => Global.BasicBus.Publish(new AddItemEvent(Factory.CreateRandomItem()));
        void OnResetClicked() => Global.BasicBus.Publish(new ResetEvent());


    }
}