using System;
using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using GDS.Core.Events;
using GDS.Basic.Views;
using static GDS.Core.Dom;
using GDS.Basic.Events;
using UnityEngine;
namespace GDS.Basic {

    public class RootLayer : VisualElement {
        public new class UxmlFactory : UxmlFactory<RootLayer> { }

        public RootLayer() {

            var Backdrop = Div("backdrop");
            var DropTarget = new DropTargetView(Store.Instance.DraggedItem, _ => Store.Bus.Publish(new DropDraggedItemEvent()));
            var SideContainer = Div("flex-1 p-50 align-items-end");
            var MainContainer = Div("flex-1 p-50 align-items-start");
            var Container = Div("window-container");

            this.Add("root-layer column",
                Backdrop,
                DropTarget,
                HelpPanel(),
                Container.Add(
                    SideContainer.IgnorePick(),
                    MainContainer.Add(
                        new InventoryView().WithClass("mb-20"),
                        DestroyItemButton()
                    ).IgnorePick()
                ).IgnorePick(),
                new LogMessageView(Store.Bus)
            )
            .WithRestrictedSlotBehavior(Store.Instance.DraggedItem)
            .WithDragToPickBehavior(Store.Instance.DraggedItem, Store.Bus)
            .WithGhostItemBehavior<BasicItemView>(Store.Instance.DraggedItem)
            .WithItemTooltipBehavior<Tooltip>();

            this.Observe(Store.Instance.IsInventoryOpen, value => {
                Container.SetVisible(value);
                Backdrop.SetVisible(value);
            });

            this.Observe(Store.Instance.SideWindow, (bag) => {
                var BagView = CreateBagView(bag);
                SideContainer.Clear();
                SideContainer.Add(BagView);
            });

            // Gamma correct
            this.Observe(Store.ColorSpace, value => {
                if (value == ColorSpace.Linear) styleSheets.Add(Resources.Load<StyleSheet>("Basic/BasicGammaCorrect"));
            });
        }

        VisualElement CreateBagView(object bag) => bag switch {
            CharacterSheet => new CharacterSheetWindow(),
            Stash b => new StashWindow(b),
            Vendor b when b.Infinite => new InfiniteShopWindow(b),
            Vendor b => new ShopWindow(b),
            CraftingBench b => new CraftingBenchView(b),
            ListBag b => new ChestWindow(b),
            _ => Div()
        };

        VisualElement HelpPanel() {
            var HelpText = Label("description", $"[Left click]: Move {"character".Orange()} / {"Interact".Green()}\n[C]: Character Sheet\n[Tab] or [I]: Toggle inventory\n[ESC]: Close all windows\n[1-5]: Consume items in Hotbar\nDrag, Click or Ctrl+Click to move items\n[Shift+Click]: Pick half a stack (stackable items)");
            var Buttons = ButtonBar();
            var ToggleButtons = Button("Toggle button bar", () => Buttons.Toggle());
            var ToggleHelpText = Button("Toggle help text", () => HelpText.Toggle());
            return Div("help-panel gap-v-8",
                Div("buttons-container",
                    Div("row", ToggleButtons, ToggleHelpText),
                    Buttons
                ),
                HelpText
            ).IgnorePick();
        }

        VisualElement ButtonBar() =>
             Div("row",
                Button("Materials", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.MaterialsShop))),
                Button("Equipment", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.EquipmentShop))),
                Button("Chest", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.Chest))),
                Button("Crafting", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.CraftingBench))),
                Button("Stash", () => Store.Bus.Publish(new OpenSideWindowEvent(Store.Instance.Stash))),
                Button($"Character [{"C".Orange()}]", () => Store.Bus.Publish(new ToggleCharacterSheet())),
                Button("mr-50", $"Inventory [{"Tab".Orange()}]", () => Store.Bus.Publish(new ToggleInventoryEvent())),
                Button("Reset", () => Store.Bus.Publish(new ResetEvent()))
            );

        VisualElement DestroyItemButton() {
            var btn = Div("action-button");
            Action<Item> action = item => {
                Store.Bus.Publish(new DestroyDraggedItemEvent());
                btn.TriggerClassAnimation("on-drop-effect");
            };
            btn.Add(
                Div("destroy-item-button"),
                new DropTargetView(Store.Instance.DraggedItem, action)
            );
            return btn;
        }
    }
}
