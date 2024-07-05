using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace GDS {
    public class SlotView : Component<Slot> {

        public SlotView() {
            this.
                Div("slot",
                    bg.WithClass("item-background"),
                    image.WithClass("item-image"),
                    quant.WithClass("quant"),
                    overlay.WithClass("cover overlay"),
                    label.WithClass("debug-label")).WithoutPointerEvents(true);


            RegisterCallback<MouseEnterEvent>(e => Global.GlobalBus.Publish(new SlotMouseOverEvent(this)));
            RegisterCallback<MouseLeaveEvent>(e => Global.GlobalBus.Publish(new SlotMouseOutEvent(this)));
        }


        Label label = new();
        VisualElement overlay = new();
        VisualElement bg = new();
        VisualElement image = new();
        Label quant = new();
        public Bag Bag;
        public Slot Slot;

        string RarityClass(Item item) => item.Rarity switch {
            ItemRarity.Magic => "item-magic",
            ItemRarity.Rare => "item-rare",
            ItemRarity.Unique => "item-unique",
            _ => "item-no-rarity"
        };

        override public void Render(Slot slot) {
            Slot = slot;
            label.text = $"[{slot.Type}]{Environment.NewLine}{slot.Item.Type}";
            if (slot.IsEmpty()) AddToClassList("empty");
            else RemoveFromClassList("empty");
            image.style.backgroundImage = new StyleBackground(slot.Item.Image());
            quant.text = slot.Item.Quant.ToString();
            quant.style.display = slot.Item.IsStackable() ? DisplayStyle.Flex : DisplayStyle.None;
            bg.style.display = slot.IsEmpty() ? DisplayStyle.None : DisplayStyle.Flex;
            bg.ClearClassList();
            bg.WithClass("item-background " + RarityClass(slot.Item));
        }

        public void ClearDropTargetVisual() {
            RemoveFromClassList("valid-drop-target");
            RemoveFromClassList("invalid-drop-target");
        }

        public void SetDropTargetVisual(bool valid) {
            if (valid) AddToClassList("valid-drop-target");
            else AddToClassList("invalid-drop-target");
        }
    }
}