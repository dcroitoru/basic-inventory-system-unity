using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using GDS.Core.Events;
using GDS.Basic.Views;
using GDS.Basic.Views.Components;
using static GDS.Core.Dom;
namespace GDS.Basic {

    public class RootLayer : VisualElement {

        public RootLayer() {

            var Background = new DropTargetView();
            var SideContainer = Div("flex-1 p-50 align-items-end");
            var MainContainer = Div("flex-1 p-50 align-items-start");
            var Container = Div("window-container");

            this.Add("root-layer column",
                Background,
                HelpPanel(),
                Container.Add(
                    SideContainer.IgnorePick(),
                    MainContainer.Add(
                        new InventoryView().WithClass("mb-20"),
                        DestroyItemButton()
                    ).IgnorePick()
                ).IgnorePick(),
                new LogMessageView()
            )
            .WithRestrictedSlotBehavior(Store.Instance.DraggedItem)
            .WithDragToPickBehavior(Store.Instance.DraggedItem, Store.Bus)
            .WithGhostItemBehavior<BasicItemView>(Store.Instance.DraggedItem)
            .WithItemTooltipBehavior<Tooltip>();

            // TODO: must be able to prevent 3d scene clicks on button bar 
            this.Observe(Store.Instance.IsInventoryOpen, value => {
                Container.SetVisible(value);
                Background.SetVisible(value);
            });

            this.Observe(Store.Instance.SideWindow, (bag) => {
                var BagView = CreateBagView(bag);
                SideContainer.Clear();
                SideContainer.Add(BagView);
            });

            // TODO: should this be a behavior?
            Background.RegisterCallback<PointerUpEvent>(e => {
                if (Store.Instance.DraggedItem.Value is NoItem) return;
                Store.Bus.Publish(new DropDraggedItemEvent());
            });

        }

        VisualElement CreateBagView(Bag bag) => bag switch {
            Stash b => new StashWindow(b),
            Vendor b => new VendorWindow(b),
            CraftingBench b => new CraftingBenchView(b),
            ListBag b => new ChestWindow(b),
            _ => Div()
        };

        VisualElement HelpPanel() {
            var HelpText = Label("description", $"[Left click]: Move {"character".Orange()} / {"Interact".Green()}\n[Tab] or [I]: Toggle inventory\n[ESC]: Close all windows\n[1-5]: Consume items in Hotbar\nDrag, Click or Ctrl+Click to move items\n[Shift+Click]: Pick half a stack (stackable items)");
            var Buttons = ButtonBar();
            var ToggleButtons = Button("Toggle button bar", () => Buttons.Toggle());
            var ToggleHelpText = Button("Toggle help text", () => HelpText.Toggle());
            return Div("help-panel gap-v-8",
                Div("buttons-container",
                    Div("row", ToggleButtons, ToggleHelpText),
                    Buttons
                ),
                HelpText
            );
        }

        VisualElement ButtonBar() =>
             Div("row",
                Button("Vendor", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.Vendor))),
                Button("Chest", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.Chest))),
                Button("Crafting", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.CraftingBench))),
                Button("Stash", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.Stash))),
                Button("mr-50", $"Inventory [{"Tab".Blue()}]", () => Store.Bus.Publish(new ToggleInventoryEvent())),
                Button("Reset", () => Store.Bus.Publish(new ResetEvent()))
            );

        VisualElement DestroyItemButton() {
            var btn = Div("drop-target", Div("destroy-item-button"));
            btn.RegisterCallback<PointerUpEvent>(e => {
                if (Store.Instance.DraggedItem.Value is NoItem) return;
                Store.Bus.Publish(new DestroyDraggedItemEvent());
                btn.TriggerClassAnimation("on-drop-effect");
            });
            return btn;
        }
    }
}
