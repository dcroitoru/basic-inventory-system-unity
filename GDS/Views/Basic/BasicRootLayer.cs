using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Factory;
using static GDS.Dom;
namespace GDS {

    public class BasicRootLayer : VisualElement {

        public BasicRootLayer() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            var store = Global.BasicInventoryStore;
            var ghostItem = new BasicGhostItemView(this, store.Bus);
            this.
                Div("root-layer",
                    Description(),
                    ButtonBar(),
                    Div("row",
                        Div("side-bar",
                            new VendorWindow(store.vendor),
                            new ChestWindow(store.chest1)
                        ),
                        new BasicInventoryWindow()
                    ),
                    new MessageView(),
                    ghostItem
                );
        }

        // GhostItemView ghostItem;
        VisualElement ButtonBar() =>
             Div("row",
                Button("", "Toggle Vendor", () => Global.BasicBus.Publish(new ToggleWindowEvent("vendor1"))),
                Button("mr-100", "Toggle Chest", () => Global.BasicBus.Publish(new ToggleWindowEvent("chest1"))),
                Button("mr-100", "Toggle Inventory (I)", () => Global.BasicBus.Publish(new ToggleInventoryEvent())),
                Button("", "Reset", () => Global.BasicBus.Publish(new ResetEvent()))
            );

        VisualElement Description() => Label("description", "Press [I] to toggle inventory or [ESC] to close it\nPress [1-4] to consume items in the Hotbar");






    }
}
