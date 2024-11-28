using System;
using System.Linq;
using UnityEngine.UIElements;


namespace GDS.Core.Views {

    public class BaseSlotView : Component<Slot> {
        public BaseSlotView(Slot slot, Bag bag) {
            Data = slot;
            Bag = bag;
        }
        public Bag Bag { get; private set; } = Bag.NoBag;
    }

    public class SlotView : SlotView<ItemView> {
        public SlotView(Slot slot, Bag bag) : base(slot, bag) { }
    }

    /// <summary>
    /// A component that displays a slot in an inventory
    /// </summary>
    public class SlotView<T> : BaseSlotView where T : Component<Item> {

        public SlotView(Slot slot, Bag bag) : base(slot, bag) {
            this.Add("slot",
                debugLabel.WithClass("debug-label").Hide(),
                overlay.WithClass("cover overlay")
            ).WithoutPointerEventsInChildren();
        }

        Label debugLabel = new();
        VisualElement overlay = new();
        T itemView = Activator.CreateInstance<T>();


        override public void Render(Slot slot) {

            debugLabel.text = $"[{slot.Item.Name()}]";
            if (slot.IsEmpty()) {
                if (Children().Contains(itemView)) Remove(itemView);
                AddToClassList("empty");
                return;
            }

            RemoveFromClassList("empty");
            itemView.Data = slot.Item;
            Insert(0, itemView.WithoutPointerEvents());
        }
    }
}