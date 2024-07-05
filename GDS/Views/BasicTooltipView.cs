
using System.Collections.Generic;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

using static GDS.Util;
using static GDS.Factory;
using static GDS.Dom;


namespace GDS {

    public class BasicTooltipView : VisualElement {
        public BasicTooltipView(VisualElement root) {

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicTooltip"));

            this.root = root;
            Tooltip = Dom.Div("tooltip");
            this.Div("tooltip-container", Tooltip);
            this.Hide();


            Global.GlobalBus.Subscribe<SlotMouseOverEvent>(e => onSlotMouseOver(e as SlotMouseOverEvent));
            Global.GlobalBus.Subscribe<SlotMouseOutEvent>(e => onSlotMouseOut(e as SlotMouseOutEvent));
            Global.GlobalBus.Subscribe<Invalidate>(e => this.Hide());

        }

        VisualElement root;
        VisualElement Tooltip;
        CancellationTokenSource cts = new();

        string RarityClass(ItemRarity rarity) => rarity switch {
            ItemRarity.Unique => "unique",
            ItemRarity.Rare => "rare",
            ItemRarity.Magic => "magic",
            _ => "common"
        };

        public void Render(Item item) {
            Tooltip.Clear();
            Tooltip.Div(ItemName(item)).WithoutPointerEvents();
        }

        void SetPosition(Rect bounds) {

            var topOffset = root.worldBound.yMin;
            float left, top;
            top = bounds.yMin - worldBound.height - topOffset;
            left = bounds.center.x - worldBound.width / 2;

            if (top < 0) {
                top = 0;
                left = bounds.xMin - worldBound.width;
                if (left < 0) {
                    left = bounds.xMax;
                }
            } else {
                if (left < 0) {
                    left = 0;
                }
                if (left + worldBound.width > root.worldBound.width) {
                    left = root.worldBound.width - worldBound.width;
                }
            }

            style.left = left;
            style.top = top;
            style.visibility = Visibility.Visible;
        }

        async void onSlotMouseOver(SlotMouseOverEvent e) {
            if (e.SlotView.Slot.IsEmpty()) return;

            Render(e.SlotView.Slot.Item);
            style.visibility = Visibility.Hidden;
            this.Show();
            cts.Cancel();
            cts = new CancellationTokenSource();
            try {
                await Task.Delay(75, cts.Token);
                SetPosition(e.SlotView.worldBound);
            }
            catch (OperationCanceledException) { }

        }

        void onSlotMouseOut(SlotMouseOutEvent e) {
            // Log("should hide tooltip");
            this.Hide();
        }

        VisualElement ItemName(Item item) {
            var quantString = item.Quant > 1 ? $" ({item.Quant})" : "";
            return Label($"name {RarityClass(item.Rarity)}", item.Type.ToString() + quantString);
        }



    }
}