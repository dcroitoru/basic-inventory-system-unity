using GDS.Core.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core.Views {

    public class Tooltip : Component<Item> {
        public Tooltip(VisualElement root) {
            styleSheets.Add(Resources.Load<StyleSheet>("Shared/Styles/BasicTooltip"));
            this.root = root;
            this.Add("tooltip", ItemName.WithClass("tooltip__item-name"));
        }

        public readonly VisualElement root;
        public readonly Label ItemName = new();

        override public void Render(Item item) {
            ItemName.text = item.Name();
        }
    }

    public static class TooltipExt {


        public static void SetPosition(this Tooltip tooltip, Rect targetBounds, Rect worldBounds) {

            var topOffset = worldBounds.yMin;
            float left, top;
            top = targetBounds.yMin - tooltip.worldBound.height - topOffset;
            left = targetBounds.center.x - tooltip.worldBound.width / 2;

            if (top < 0) {
                top = 0;
                left = targetBounds.xMin - tooltip.worldBound.width;
                if (left < 0) {
                    left = targetBounds.xMax;
                }
            } else {
                if (left < 0) {
                    left = 0;
                }
                if (left + tooltip.worldBound.width > tooltip.root.worldBound.width) {
                    left = tooltip.root.worldBound.width - tooltip.worldBound.width;
                }
            }

            tooltip.style.left = left;
            tooltip.style.top = top;
            tooltip.style.visibility = Visibility.Visible;
        }
    }
}