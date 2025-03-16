using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using GDS.Basic.Views;
using static GDS.Core.Dom;
using System.Linq;


namespace GDS.Basic {

    public class CharacterSheetWindow : VisualElement {
        public new class UxmlFactory : UxmlFactory<CharacterSheetWindow> { }
        public CharacterSheetWindow() {
            var store = Store.Instance;

            var Defense = Label("character-line", "Defense");
            var DPS = Label("character-line", "DPS");
            this.Add("window character-sheet",
                Components.CloseButton(store.Main),
                Title("Character Sheet"),
                Div(
                    Defense,
                    DPS
                )
            ).Gap(50);

            this.Observe(Store.Instance.CharacterSheet.Stats, (data) => {
                Defense.text = $"Defense: {DefenseStr(data.Defense)}";
                DPS.text = $"Damage: {DamageStr(data.Damage)}" + "/s".DarkGray();
            });

        }

        string DefenseStr(int value) => value switch {
            <= 40 => value.ToString().Red(),
            <= 70 => value.ToString().Yellow(),
            _ => value.ToString().Blue()
        };

        string DamageStr(float value) => value switch {
            <= 0 => value.ToString().Red(),
            _ => value.ToString().Green()
        };

    }

}
