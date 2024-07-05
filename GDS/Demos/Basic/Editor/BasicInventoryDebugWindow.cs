using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using GDS;
using static GDS.Dom;
using GDS.Demos.Basic;
namespace GDS.Demos.Basic {

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
                    new InventoryView(),
                    Button("", "Add Random Item", () => OnAddClicked(ItemType.Dagger)),
                    Button("", "Reset", () => OnResetClicked()),
                    new GhostItemView(root, Store.Instance.draggedItem, Store.Bus),
                    new BasicTooltipView(root)
            );
        }

        void OnAddClicked(ItemType type) => Store.Bus.Publish(new AddItemEvent(Factory.CreateRandomItem()));
        void OnResetClicked() => Store.Bus.Publish(new ResetEvent());

    }
}