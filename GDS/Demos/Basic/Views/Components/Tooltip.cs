using GDS.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Basic.Views.Components {

    public class Tooltip : Core.Views.Tooltip {
        public Tooltip(VisualElement root) : base(root) {
            this.Add(
                ItemRarity.WithClass("tooltip__affix-text"),
                Defense.WithClass("tooltip__affix-text"),
                Attack.WithClass("tooltip__affix-text"),
                AttackSpeed.WithClass("tooltip__affix-text"),
                DPS.WithClass("tooltip__affix-text")
            );
        }

        public readonly Label ItemRarity = new();
        public readonly Label Defense = new();
        public readonly Label Attack = new();
        public readonly Label AttackSpeed = new();
        public readonly Label DPS = new();

        public override void Render(Item item) {
            base.Render(item);
            Defense.Hide();
            Attack.Hide();
            AttackSpeed.Hide();
            DPS.Hide();
            ItemRarity.Hide();
            ClearClassList();
            this.WithClass("tooltip " + RarityClass(item.Rarity()));

            if (item.Rarity() is not Rarity.NoRarity) {
                ItemRarity.text = $"[{item.Rarity()}]";
                ItemRarity.Show();
            }

            switch (item.ItemBase) {
                case ArmorItemBase itemBase:
                    Defense.text = "Defense: " + itemBase.Defense.ToString().Blue();
                    Defense.Show();
                    break;
                case WeaponItemBase itemBase:
                    Attack.text = "Attack: " + itemBase.Attack.ToString().Green();
                    AttackSpeed.text = "AttackSpeed: " + itemBase.AttackSpeed.ToString().Green();
                    DPS.text = "DPS: " + (itemBase.Attack * itemBase.AttackSpeed).ToString().Pink();
                    Attack.Show();
                    AttackSpeed.Show();
                    DPS.Show();
                    break;
                default: break;
            }
        }

        string RarityClass(Rarity rarity) => rarity switch {
            Rarity.Unique => "unique",
            Rarity.Rare => "rare",
            Rarity.Magic => "magic",
            _ => "common"
        };
    }

}