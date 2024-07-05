using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace GDS {

    public class ListInventoryView<T> : SmartComponent<T> where T : Inventory {
        public ListInventoryView(T bag) : base(bag) {
            slots = bag.Slots.Select(x => createSlot(x, bag)).ToArray();
            this.Div("inventory", Dom.Div("slot-container", slots));
        }

        SlotView[] slots;
        SlotView createSlot(Slot slot, Bag bag) => new() { Data = slot, Bag = bag };

        override public void Render(T bag) {
            // Util.Log("should render bag".blue(), bag.GetType().ToString().green());
            for (var i = 0; i < bag.Slots.Count; i++) slots[i].Data = bag.Slots[i];
        }

    }
}
