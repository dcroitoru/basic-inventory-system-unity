using System;
using System.Linq;
using UnityEngine.UIElements;

namespace GDS.Core.Views {
    /// <summary>
    /// A smart component that displays a list of slots
    /// </summary>
    public class ListBagView<T> : VisualElement where T : BaseSlotView {
        public ListBagView(ListBag bag) {
            // createSlotFn ??= (Slot slot, Bag bag) => new SlotView(slot, bag);
            T createSlotFn(ListSlot slot) => (T)Activator.CreateInstance(typeof(T), slot, bag);
            var slotViews = bag.Slots.Select(slot => createSlotFn(slot)).ToArray();
            this.Add("list-bag", Dom.Div("slot-container", slotViews));
            this.Observe(bag.Data, (slots) => {
                for (var i = 0; i < slots.Count; i++) slotViews[i].Data = slots[i];
            });
        }
    }
}
