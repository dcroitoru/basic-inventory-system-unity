using UnityEngine;
using UnityEditor;
using GDS.Core;
using GDS.Core.Views;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Sample;
using static GDS.Core.Dom;

namespace GDS.Minimal {

    public class DebugWindow : EditorWindow {
        [MenuItem("Tools/Minimal/Debug")]
        public static void Open() => GetWindow<DebugWindow>().titleContent = new GUIContent("Minimal::Debug");

        public void CreateGUI() {
            var root = rootVisualElement;
            root.styleSheets.Add(Resources.Load<StyleSheet>("Minimal/MinimalStyles"));

            var draggedItem = Label(Store.Instance.DraggedItem.Value.Name());
            root.Div("debug-window",
                Dom.Div(
                    Dom.Title("Dragged item"),
                    draggedItem
                ),
                new InventoryDebug(Store.Instance.MainInventory)
            );

            Store.Instance.DraggedItem.OnChange += (item) => draggedItem.text = item.Name();
        }

    }

    public class InventoryDebug : SmartComponent<ListBag> {
        public InventoryDebug(ListBag e) : base(e) { }

        string CreateText(ListSlot slot) {
            var str = $"{slot.Index}: {slot.Item.Name()} ({slot.Item.Rarity()})";
            return str;
        }

        public override void Render(ListBag data) {
            Clear();
            this.Div(
                Dom.Label("title", data.Id),
                Div(data.Slots
                        .Select((slot) => CreateText(slot))
                        .Select(str => Label(str))
                        .ToArray())
            );
        }
    }


}