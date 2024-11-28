using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using GDS.Core.Views;
using GDS.Core;

namespace GDS.Basic.Views {

    public class BasicSlotView : SlotView<BasicItemView> { public BasicSlotView(Slot slot, Bag bag) : base(slot, bag) { } }

    public class BasicListBagView<T> : SmartComponent<T> where T : ListBag {
        public BasicListBagView(T bag) : base(bag) {
            slots = bag.Slots.Select(x => new BasicSlotView(x, bag)).ToArray();
            this.Add("inventory",
                Dom.Div("slot-container", slots)
            );
        }

        BasicSlotView[] slots;

        override public void Render(T bag) {
            for (var i = 0; i < bag.Slots.Count; i++) slots[i].Data = bag.Slots[i];
        }

    }
}
