using System;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Core.Views;
using GDS.Core;
using GDS.Core.Events;
using GDS.Basic.Views;
using static GDS.Core.Dom;
namespace GDS.Basic {

    public class ChestWindow : SmartComponent<ListBag> {

        public ChestWindow(ListBag chest, string Title = "Chest (Remove Only)") : base(chest) {

            Action onCollectClick = () => {
                var items = chest.Slots.Select(slot => slot.Item).ToArray();
                bus.Publish(new CollectAllEvent(chest, items));
            };

            slotContainer = Div("slot-container mb-10");
            content = Div("chest column",
                slotContainer,
                Button("collect-button", "Collect all", onCollectClick)
            );
            empty = Label("empty-message", "[Empty]");

            this.Div("window",
                Dom.Title(Title),
                content,
                empty
            ).WithWindowBehavior(chest.Id, Store.Instance.sideWindowId, Store.Bus);
        }

        EventBus bus = Store.Bus;
        VisualElement slotContainer;
        VisualElement content;
        Label empty;

        override public void Render(ListBag chest) {
            if (chest.IsEmpty()) {
                content.Hide();
                empty.Show();
                return;
            }

            empty.Hide();
            content.Show();
            slotContainer.Clear();
            slotContainer.Div(chest.Slots.Where(InventoryExtensions.IsNotEmpty).Select(x => new BasicSlotView(x, chest)).ToArray());
        }


    }
}
