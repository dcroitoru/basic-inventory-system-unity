using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;


namespace GDS.Core.Views {

    public class BaseSetSlotView : Component<SetSlot> {
        public BaseSetSlotView(SetSlot slot, Bag bag) {
            Data = slot;
            Bag = bag;
        }
        public Bag Bag { get; private set; } = Bag.NoBag;
    }

    public class SetSlotView : SetSlotView<ItemView> {
        public SetSlotView(SetSlot slot, Bag bag) : base(slot, bag) { }
    }

    /// <summary>
    /// A component that displays a named slot in an inventory (i.e. an equipment slot)
    /// </summary>
    public class SetSlotView<T> : BaseSetSlotView where T : Component<Item> {

        public SetSlotView(SetSlot slot, Bag bag) : base(slot, bag) {
            this.Add("slot",
                itemView,
                debugLabel.WithClass("debug-label"),
                overlay.WithClass("cover overlay")
            )
            .WithoutPointerEventsInChildren();
        }

        Label debugLabel = new();
        VisualElement overlay = new();
        T itemView = Activator.CreateInstance<T>();

        override public void Render(SetSlot slot) {
            debugLabel.text = $"[{slot.Key}]\n[{slot.Item.ItemBase.Id}]";
            if (slot.IsEmpty()) {
                if (Children().Contains(itemView)) Remove(itemView);
                AddToClassList("empty");
                return;
            }

            RemoveFromClassList("empty");
            itemView.Data = slot.Item;
            Insert(0, itemView);
        }
    }
}