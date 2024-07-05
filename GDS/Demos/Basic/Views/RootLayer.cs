using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Factory;
using static GDS.Dom;
namespace GDS.Demos.Basic {

    public class RootLayer : VisualElement {

        public RootLayer() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            var ghostItem = new GhostItemView(this, Store.Instance.draggedItem, Store.Bus);
            var tooltip = new BasicTooltipView(this);
            this.Div("root-layer",
                Description(),
                ButtonBar(),
                Div("row",
                    Div("side-bar",
                        new VendorWindow(Store.Instance.vendor),
                        new ChestWindow(Store.Instance.chest1)
                    ),
                    new InventoryView()
                ),
                new MessageView(),
                tooltip,
                ghostItem
            );
        }

        // GhostItemView ghostItem;
        VisualElement ButtonBar() =>
             Div("row",
                Button("Toggle Vendor", () => Store.Bus.Publish(new ToggleWindowEvent("vendor1"))),
                Button("mr-100", "Toggle Chest", () => Store.Bus.Publish(new ToggleWindowEvent("chest1"))),
                Button("mr-100", "Toggle Inventory (I)", () => Store.Bus.Publish(new ToggleInventoryEvent())),
                Button("Reset", () => Store.Bus.Publish(new ResetEvent()))
            );

        VisualElement Description() => Label("description", "Press [I] to toggle inventory or [ESC] to close it\nPress [1-4] to consume items in the Hotbar");






    }
}
