using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


namespace GDS.Core.Views {

    public interface ISlotView {
        public Slot Data { get; }
        public Bag Bag { get; }
        public Rect Bounds { get; }
        public Rect GetBounds();
    }

    public class BaseSlotView : Component<Slot>, ISlotView {
        public BaseSlotView(Slot slot, Bag bag) { Data = slot; Bag = bag; Bounds = worldBound; }
        public Bag Bag { get; private set; }
        public Rect Bounds { get; private set; }
        public Rect GetBounds() {
            return new Rect();
        }
    }

    /// <summary>
    /// A component that displays a slot in an inventory
    /// </summary>
    public class SlotView<T> : BaseSlotView where T : Component<Item> {

        public SlotView(Slot slot, Bag bag) : base(slot, bag) {
            this.Add("slot",
                debug.WithClass("debug-label"),
                itemView,
                overlay.WithClass("cover overlay")
            ).IgnorePickChildren();
        }

        Label debug = new();
        VisualElement overlay = new();
        T itemView = Activator.CreateInstance<T>();

        override public void Render(Slot slot) {
            debug.text = $"[{slot.Item.Name()}]";
            itemView.Data = slot.Item;
            EnableInClassList("empty", slot.IsEmpty());
        }
    }
}