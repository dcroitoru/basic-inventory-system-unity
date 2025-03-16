using System.Linq;
using GDS.Core;
using UnityEngine;

namespace GDS.Basic {
    public class CharacterSheet {
        public Observable<CharacterStats> Stats;
        public CharacterSheet(SetBag bag) {
            Stats = new(new(0, 0));

            bag.Data.OnChange += (data) => {
                var defense = 0;
                var dps = 0f;
                for (var i = 0; i < data.Count; i++) {
                    if (data.ElementAt(i).Value.Item.ItemBase is ArmorItemBase a) defense += a.Defense;
                    if (data.ElementAt(i).Value.Item.ItemBase is WeaponItemBase b) dps += b.Attack * b.AttackSpeed;
                }
                Stats.SetValue(Stats.Value with { Defense = defense, Damage = dps });
            };
        }

    }
}