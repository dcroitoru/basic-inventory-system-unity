using System;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Core.Views;
using GDS.Core;
using GDS.Core.Events;
using GDS.Basic.Views;
using static GDS.Core.Dom;
namespace GDS.Basic {

    public class ChestWindow : VisualElement {

        public ChestWindow(ListBag chest, string titleText = "Chest (Remove Only)") {

            Action onCollectClick = () => {
                var items = chest.Slots.Select(slot => slot.Item).ToArray();
                Store.Bus.Publish(new CollectAllEvent(chest, items));
            };

            var slotContainer = Div("slot-container");
            var content = Div("chest column",
                slotContainer,
                Button("collect-button", "Collect all", onCollectClick)
            );
            var emptyLabel = Label("empty-message", "[Empty]");

            this.Add("window",
                Comps.CloseButton(chest),
                Title(titleText),
                emptyLabel,
                content
            );

            this.Observe(chest.Data, (_) => {
                if (chest.IsEmpty()) {
                    content.Hide();
                    emptyLabel.Show();
                    return;
                }

                emptyLabel.Hide();
                content.Show();
                slotContainer.Clear();
                slotContainer.Div(chest.Slots.Where(InventoryExtensions.IsNotEmpty).Select(x => new BasicSlotView(x, chest)).ToArray());
            });

        }
    }
}
