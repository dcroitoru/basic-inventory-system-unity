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


            var defaultState = Div("container",
                new ListBagView<SlotView<BasicItemView>>(chest),
                Button("collect-button", "Collect all", onCollectClick)
            );
            var emptyState = Label("empty-message", "[Empty]");

            this.Add("window chest",
                Components.CloseButton(chest),
                Title(titleText),
                emptyState,
                defaultState
            );

            this.Observe(chest.Data, (_) => {
                if (chest.IsEmpty()) {
                    defaultState.Hide();
                    emptyState.Show();
                    return;
                }

                emptyState.Hide();
                defaultState.Show();
            });

        }
    }
}
