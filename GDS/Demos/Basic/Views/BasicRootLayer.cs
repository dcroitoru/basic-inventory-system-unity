using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Core.Dom;
using GDS.Core;
using GDS.Core.Views;
using GDS.Core.Events;
using GDS.Basic.Views;
namespace GDS.Basic {

    public class RootLayer : VisualElement {

        public RootLayer() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            styleSheets.Add(Resources.Load<StyleSheet>("Shared/Styles/TW"));

            var ghostItem = new GhostItemView<BasicItemView>(this, Store.Instance.DraggedItem) { UseItemSize = false };
            var tooltipElement = new Views.Components.Tooltip(this);
            var dropItem = new Label("Drop Item") { tooltip = "Drag an item here" };
            var destroyItem = new Label("Destroy Item(!!!)") { tooltip = "Drag an item here" };

            this.Add("root-layer",
                Description().WithClass("mt-50"),
                ButtonBar(),
                Div("row",
                    Div("side-bar",
                        new VendorWindow(Store.Instance.Vendor),
                        new ChestWindow(Store.Instance.Chest),
                        new ChestWindow(Store.Instance.Ground, "Ground items (remove only)"),
                        new StashWindow(Store.Instance.Stash),
                        new CraftingBenchView(Store.Instance.CraftingBench)
                    ),
                    new InventoryView()
                ),
                Dom.Div("drop-buttons",
                    dropItem.WithClass("drop-target"),
                    destroyItem.WithClass("drop-target")
                ),
                new MessageView(),
                ghostItem,
                tooltipElement
            )
            .WithTooltipBehavior(tooltipElement)
            .WithDefaultHoverBehavior(Store.Instance.DraggedItem)
            .WithPOEItemPickBehavior(Store.Instance.DraggedItem, Store.Bus);

            dropItem.RegisterCallback<PointerUpEvent>(e => {
                if (Store.Instance.DraggedItem.Value is NoItem) return;
                Store.Bus.Publish(new DropDraggedItemEvent());
                dropItem.WithClass("on-drop-effect");
                dropItem.schedule.Execute(() => {
                    dropItem.WithoutClass("on-drop-effect");
                }).ExecuteLater(100);

            });

            destroyItem.RegisterCallback<PointerUpEvent>(e => {
                if (Store.Instance.DraggedItem.Value is NoItem) return;
                Store.Bus.Publish(new DestroyDraggedItemEvent());
                destroyItem.WithClass("on-drop-effect");
                destroyItem.schedule.Execute(() => {
                    destroyItem.WithoutClass("on-drop-effect");
                }).ExecuteLater(100);
            });

            Store.Instance.IsInventoryOpen.OnChange += (bool open) => {
                dropItem.SetVisible(open);
                destroyItem.SetVisible(open);
            };

        }



        VisualElement ButtonBar() =>
             Div("row",
                Button("Vendor", () => Store.Bus.Publish(new ToggleWindowEvent("vendor"))),
                Button("Chest", () => Store.Bus.Publish(new ToggleWindowEvent("chest"))),
                Button("Crafting", () => Store.Bus.Publish(new ToggleWindowEvent("crafting-bench"))),
                Button("Stash", () => Store.Bus.Publish(new ToggleWindowEvent("stash"))),
                Button("mr-50", "Ground", () => Store.Bus.Publish(new ToggleWindowEvent("ground"))),
                Button("mr-50", "Inventory (I)", () => Store.Bus.Publish(new ToggleInventoryEvent())),
                Button("Reset", () => Store.Bus.Publish(new ResetEvent()))
            );

        VisualElement Description() => Label("description", "Press [I] to toggle inventory or [ESC] to close it\nPress [1-4] to consume items in the Hotbar");





    }
}
