using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Factory;
using static GDS.Dom;
using static GDS.Util;
namespace GDS {

    public class ChestWindow : SmartComponent<Chest> {

        public ChestWindow(Observable<Chest> chest) : base(chest) {
            this.chest = chest.Value;
            var store = Global.BasicInventoryStore;
            bus = store.Bus;
            store.sideWindowId.OnNext += SetDisplay;
            SetDisplay(store.sideWindowId.Value);

            slotContainer = Div("slot-container mb-10");
            content =
                Div("chest column",
                    slotContainer,
                    Button("collect-button", "Collect all", onCollectClick)
            );
            empty = Label("empty-message", "[Empty]");

            this.
                Div("window",
                    Label("title", "Chest (Remove Only)"),
                    content,
                    empty
            );
        }

        EventBus bus;
        Chest chest;
        VisualElement slotContainer;
        VisualElement content;
        Label empty;
        SlotView createSlot(Slot slot, Bag bag) => new() { Data = slot, Bag = bag };
        void SetDisplay(string otherId) => style.display = otherId == chest.Id ? DisplayStyle.Flex : DisplayStyle.None;

        override public void Render(Chest chest) {

            Log("should render chest".blue(), chest.ToString().green());
            if (chest.IsEmpty()) {
                content.Hide();
                empty.Show();
                return;
            }

            empty.Hide();
            content.Show();
            slotContainer.Clear();
            slotContainer.Div(chest.Slots.Where(Fn.IsNotEmpty).Select(x => createSlot(x, chest)).ToArray());
        }

        void onCollectClick() {
            Util.Log("should collect all");
            var items = chest.Slots.Select(slot => slot.Item).ToArray();
            bus.Publish(new CollectAllEvent(chest, items));
        }
    }
}
