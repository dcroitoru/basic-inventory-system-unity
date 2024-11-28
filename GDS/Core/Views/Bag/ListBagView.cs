using System.Linq;

namespace GDS.Core.Views {

    /// <summary>
    /// A smart component that displays a list of slots
    /// </summary>
    public class ListBagView<T> : SmartComponent<T> where T : ListBag {
        public ListBagView(T bag) : base(bag) {
            slots = bag.Slots.Select(slot => new SlotView(slot, bag)).ToArray();
            this.Add("inventory", Dom.Div("slot-container", slots));
        }

        SlotView[] slots;

        override public void Render(T bag) {
            for (var i = 0; i < bag.Slots.Count; i++) slots[i].Data = bag.Slots[i];
        }

    }
}
