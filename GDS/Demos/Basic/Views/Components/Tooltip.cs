using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using static GDS.Core.Dom;

namespace GDS.Basic.Views {

    public class Tooltip : Component<BagItem> {
        public Tooltip() { }

        public override void Render(BagItem data) {
            Clear();
            ClearClassList();
            this.Add("tooltip " + data.Item.Rarity(),
                CreateTooltip(data.Bag, data.Item)
            );
        }

        VisualElement CreateTooltip(Bag bag, Item item) => item.ItemBase switch {
            WeaponItemBase b => WeaponTooltip(item.Name(), item.Rarity(), b, CostString(item, bag is Vendor)),
            ArmorItemBase b => ArmorTooltip(item.Name(), item.Rarity(), b, CostString(item, bag is Vendor)),
            _ => OtherTooltip(item.Name(), item.Rarity(), item.Quant(), CostString(item, bag is Vendor))
        };

        // weapon tooltip - name, rarity, attack, attack speed, dps
        VisualElement WeaponTooltip(string name, Rarity rarity, WeaponItemBase itemBase, string cost) => Div(
            Label("tooltip__item-name", name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity),
            Label("tooltip__affix-text", "Attack: " + itemBase.Attack.ToString().Green()),
            Label("tooltip__affix-text", "AttackSpeed: " + itemBase.AttackSpeed.ToString().Green()),
            Label("tooltip__affix-text", "DPS: " + (itemBase.Attack * itemBase.AttackSpeed).ToString().Pink()),
            Label("tooltip__item-cost", cost)
        );

        // armor tooltip - name, rarity, defense
        VisualElement ArmorTooltip(string name, Rarity rarity, ArmorItemBase itemBase, string cost) => Div(
            Label("tooltip__item-name", name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity),
            Label("tooltip__affix-text", "Defense: " + itemBase.Defense.ToString().Blue()),
            Label("tooltip__item-cost", cost)
        );

        // Other tooltip - name, rarity
        VisualElement OtherTooltip(string name, Rarity rarity, int quant, string cost) => Div(
            Label("tooltip__item-name", quant > 1 ? $"{name} ({quant})" : name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity),
            Label("tooltip__item-cost", cost)
        );

        string CostString(Item item, bool isVendor) => isVendor ? "Cost: " + item.Cost() : "Sell value: " + item.SellValue();
    }

}