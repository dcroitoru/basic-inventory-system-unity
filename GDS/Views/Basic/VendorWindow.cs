using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Factory;
using static GDS.Dom;
namespace GDS {

    public class VendorWindow : VisualElement {

        public VendorWindow(Vendor vendor) {
            this.vendor = vendor;
            var store = Global.BasicInventoryStore;
            store.sideWindowId.OnNext += SetDisplay;
            SetDisplay(store.sideWindowId.Value);
            Render(vendor);

            this.
                Div("window",
                    Label("title", "Vendor"),
                    slotContainer.WithClass("slot-container mb-10"),
                    Button("", "Buy Random", BuyRandom)
            );
        }

        Vendor vendor;
        VisualElement slotContainer = new();
        SlotView createSlot(Slot slot, Bag bag) => new() { Data = slot, Bag = bag };
        void SetDisplay(string otherId) => style.display = otherId == vendor.Id ? DisplayStyle.Flex : DisplayStyle.None;

        void Render(Vendor vendor) {
            var slots = vendor.Slots.Where(Fn.IsNotEmpty).Select(x => createSlot(x, vendor));
            slotContainer.Div(slots.ToArray());
        }

        void BuyRandom() {
            var item = CreateRandomItem();
            Global.BasicBus.Publish(new AddItemEvent(item));
            Global.GlobalBus.Publish(new MessageEvent($"Bought one <color=#5490e4>[{item.Type}]"));
        }


    }
}
