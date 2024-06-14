using System;
using UnityEngine.UIElements;

namespace GDS {
    public class SlotView : Component<Slot> {

        public SlotView() {

            this.
                Div("slot",
                    image.WithClass("item-image").WithPickIgnore(),
                    quant.WithClass("quant").WithPickIgnore(),
                    overlay.WithClass("cover overlay").WithPickIgnore(),
                    label.WithClass("debug-label").WithPickIgnore());

            RegisterCallback<MouseEnterEvent>(e => Global.GlobalBus.Publish(new SlotHoverEvent(this)));
        }


        Label label = new();
        VisualElement overlay = new();
        VisualElement image = new();
        Label quant = new();
        public Bag Bag;

        override public void Render(Slot slot) {
            label.text = $"[{slot.Type}]{Environment.NewLine}{slot.Item.Type}";
            if (slot.IsEmpty()) AddToClassList("empty");
            else RemoveFromClassList("empty");
            image.style.backgroundImage = new StyleBackground(slot.Item.Image());
            quant.text = slot.Item.Quant.ToString();
            quant.style.display = slot.Item.IsStackable() ? DisplayStyle.Flex : DisplayStyle.None;
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