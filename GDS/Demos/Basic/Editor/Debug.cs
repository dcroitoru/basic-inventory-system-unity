using UnityEngine;
using UnityEditor;
using GDS.Core;
using static GDS.Core.Dom;
using GDS.Core.Views;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Sample;
using System.Collections.Generic;
using System;

namespace GDS.Basic {

    public class DebugWindow : EditorWindow {
        [MenuItem("Tools/Basic/Debug")]
        public static void Open() {
            var wnd = GetWindow<DebugWindow>();
            wnd.titleContent = new GUIContent("Basic::Debug");
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            root.styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));

            root.Add("debug-window row",
                Dom.Div("gap-v-20",
                    CreateDragItemDebug(),
                    new EquipmentDebug(Store.Instance.Equipment),
                    new InventoryDebug(Store.Instance.Inventory)
                ),
                Dom.Div("gap-v-20",
                    new InventoryDebug(Store.Instance.Ground)
                )
            );
        }

        VisualElement CreateDragItemDebug() {
            var el = Dom.Div();

            Action<Item> render = (Item item) => {
                Func<string, string> fn = item switch {
                    NoItem => ColorUtil.Gray,
                    _ => (string x) => x
                };
                el.Clear();
                el.Div(
                    Dom.Title("Dragged item"),
                    Dom.Label(fn($"{item.Name()} [{item.ItemData.Quant}]"))
                );
            };

            render(Store.Instance.DraggedItem.Value);

            Store.Instance.DraggedItem.OnChange += render;

            return el;
        }

    }

    public class EquipmentDebug : SmartComponent<SetBag> {
        public EquipmentDebug(SetBag e) : base(e) { }

        string CreateText(SetSlot slot) {
            var str = $"{slot.Key}: {slot.Item.Name()} ({slot.Item.Rarity()})";
            return str;
        }

        public override void Render(SetBag data) {
            Clear();
            this.Div(
                Dom.Label("title", data.Id),
                Div(data.Slots.Values.Select(CreateText).Select(str => Label(str)).ToArray())
            );
        }
    }

    public class InventoryDebug : SmartComponent<ListBag> {
        public InventoryDebug(ListBag e) : base(e) { }

        string CreateText(ListSlot slot) {
            var str = $"{slot.Index}: {slot.Item.ItemBase.Name} [{slot.Item.ItemData.Quant}] [id:{slot.Item.Id}] ({slot.Item.Rarity()})";
            str = slot.Item switch {
                NoItem => str.Gray(),
                _ => str
            };
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