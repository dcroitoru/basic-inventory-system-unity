using System;
using System.Linq;
using GDS.Core;
using GDS.Core.Events;
using GDS.Core.Views;
using UnityEngine;
using static GDS.Core.Dom;

namespace GDS.Basic {

    public class VendorWindow : SmartComponent<Vendor> {

        public VendorWindow(Vendor bag) : base(bag) {

            Action BuyRandom = () => {
                var index = UnityEngine.Random.Range(0, bag.NonEmptySlots().ToList().Count);
                var item = bag.Slots[index].Item;
                Store.Bus.Publish(new BuyItemEvent(item));
            };

            var slots = bag.Slots.Where(InventoryExtensions.IsNotEmpty).Select(x => new SlotView(x, bag));

            this.Add("window",
                Title("Vendor (Infinite resources)"),
                Div("slot-container mb-10", slots.ToArray()),
                Button("", "Buy Random", BuyRandom)
            ).WithWindowBehavior(bag.Id, Store.Instance.sideWindowId, Store.Bus);

        }


    }
}
