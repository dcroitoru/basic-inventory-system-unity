using UnityEngine.UIElements;
using GDS.Core.Views;
using GDS.Core;

namespace GDS.Basic.Views {
    public class BasicItemView : Component<Item> {

        public BasicItemView() {

            this.Add("item",
                bg.WithClass("item-background"),
                image.WithClass("item-image"),
                quant.WithClass("item-quant"),
                debug.WithClass("debug-label")
            ).IgnorePickChildren();
        }

        VisualElement bg = new();
        VisualElement image = new();
        Label quant = new();
        Label debug = new();

        override public void Render(Item item) {
            debug.text = $"[{item.Name()}]\n[{item.Rarity()}]";
            var rarityClassname = RarityClassName(item.Rarity());
            bg.ClearClassList();
            bg.WithClass("item-background " + rarityClassname);
            image.style.backgroundImage = new StyleBackground(item.Image());
            quant.text = item.ItemData.Quant.ToString();
            quant.style.display = item.ItemBase.Stackable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public string RarityClassName(Rarity rarity) => rarity switch {
            Rarity.Unique => "item-rarity__unique",
            Rarity.Rare => "item-rarity__rare",
            Rarity.Magic => "item-rarity__magic",
            _ => "item-rarity__no-rarity"
        };
    }
}