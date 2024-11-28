using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using GDS.Core.Events;
using GDS.Core.Views;
using GDS.Core;
using GDS.Sample;

namespace GDS.Basic.Views {
    public class BasicItemView : Component<Item> {

        public BasicItemView() {

            this.Add("item",
                bg.WithClass("item-background"),
                image.WithClass("item-image"),
                quant.WithClass("item-quant"),
                debug.WithClass("debug-label").Hide()
            ).WithoutPointerEventsInChildren();
        }

        VisualElement bg = new();
        VisualElement image = new();
        Label quant = new();
        Label debug = new();

        override public void Render(Item item) {
            debug.text = $"[{item.Name()}]\n[{item.Rarity()}]\n[Stack: {item.ItemBase.Stackable}]";
            var rarityClassname = BasicItemViewExt.RarityClassName(item.Rarity());
            bg.ClearClassList();
            bg.WithClass("item-background " + rarityClassname);
            image.style.backgroundImage = new StyleBackground(item.Image());
            quant.text = item.ItemData.Quant.ToString();
            quant.style.display = item.ItemBase.Stackable ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public static class BasicItemViewExt {
        public static string RarityClassName(Rarity rarity) => rarity switch {
            Rarity.Unique => "item-rarity__unique",
            Rarity.Rare => "item-rarity__rare",
            Rarity.Magic => "item-rarity__magic",
            _ => "item-rarity__no-rarity"
        };
    }
}