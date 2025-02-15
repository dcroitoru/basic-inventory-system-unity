using System;
using System.Linq;
using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Events;
using GDS.Core.Views;
using UnityEngine.UIElements;
using static GDS.Core.Dom;

namespace GDS.Basic {

    public class VendorWindow : VisualElement {

        public VendorWindow(Vendor bag) {

            Action BuyRandom = () => {
                var index = UnityEngine.Random.Range(0, bag.NonEmptySlots().ToList().Count);
                var item = bag.Slots[index].Item;
                Store.Bus.Publish(new BuyItemEvent(item));
            };

            var slots = bag.Slots.Where(InventoryExtensions.IsNotEmpty).Select(x => new SlotView(x, bag));
            var dropTarget = new DropTargetView();

            this.Add("window vendor",
                dropTarget,
                Comps.CloseButton(bag),
                Title("Vendor (Infinite resources)"),
                Div("slot-container mb-10", slots.ToArray()).IgnorePick(),
                Button("", "Buy Random", BuyRandom)
            );

            dropTarget.RegisterCallback<PointerUpEvent>(e => {
                var item = Store.Instance.DraggedItem.Value;
                if (item is NoItem) return;
                Store.Bus.Publish(new SellItemEvent(item));
            });

        }
    }
}
