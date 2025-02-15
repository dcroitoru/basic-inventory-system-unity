using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using static GDS.Core.Dom;

namespace GDS.Basic.Views.Components {

    public class Tooltip : Component<Item> {
        public Tooltip() { }

        public override void Render(Item item) {
            Clear();
            ClearClassList();
            this.Add("tooltip " + RarityClass(item.Rarity()),
                CreateTooltip(item)
            );
        }

        string RarityClass(Rarity rarity) => rarity switch {
            Rarity.Unique => "unique",
            Rarity.Rare => "rare",
            Rarity.Magic => "magic",
            _ => "common"
        };

        VisualElement CreateTooltip(Item item) => item.ItemBase switch {
            WeaponItemBase b => WeaponTooltip(item.Name(), item.Rarity(), b),
            ArmorItemBase b => ArmorTooltip(item.Name(), item.Rarity(), b),
            _ => OtherTooltip(item.Name(), item.Rarity())
        };

        // weapon tooltip - name, rarity, attack, attack speed, dps
        VisualElement WeaponTooltip(string name, Rarity rarity, WeaponItemBase itemBase) => Div(
            Label("tooltip__item-name", name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity),
            Label("tooltip__affix-text", "Attack: " + itemBase.Attack.ToString().Green()),
            Label("tooltip__affix-text", "AttackSpeed: " + itemBase.AttackSpeed.ToString().Green()),
            Label("tooltip__affix-text", "DPS: " + (itemBase.Attack * itemBase.AttackSpeed).ToString().Pink())
        );

        // armor tooltip - name, rarity, defense
        VisualElement ArmorTooltip(string name, Rarity rarity, ArmorItemBase itemBase) => Div(
            Label("tooltip__item-name", name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity),
            Label("tooltip__affix-text", "Defense: " + itemBase.Defense.ToString().Blue())
        );

        // Other tooltip - name, rarity
        VisualElement OtherTooltip(string name, Rarity rarity) => Div(
            Label("tooltip__item-name", name),
            Label("tooltip__affix-text", $"[{rarity}]").SetVisible(rarity is not Rarity.NoRarity)
        );
    }

}