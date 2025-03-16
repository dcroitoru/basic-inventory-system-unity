using UnityEngine.UIElements;

namespace GDS.Core.Views {
    public class TooltipView : Component<Item> {
        public TooltipView() {
            this.Add("tooltip", ItemName.WithClass("tooltip__item-name"));
        }
        public Label ItemName = new();
        override public void Render(Item item) {
            ItemName.text = item.Name();
        }
    }
}