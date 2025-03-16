using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using GDS.Basic.Views;
using static GDS.Core.Dom;


namespace GDS.Basic {

    public class InventoryView : VisualElement {
        public new class UxmlFactory : UxmlFactory<InventoryView> { }
        public InventoryView() {
            var store = Store.Instance;



            this.Add("window",
                Components.CloseButton(store.Main),
                Gold(),
                Div(
                    Title("Equipment (Equipment only)"),
                    new EquipmentView(store.Equipment)),
                Div(
                    Title("Inventory (Unrestricted)"),
                    new ListBagView<BasicSlotView>(store.Main)),
                Div(
                    Title("Hotbar (Consumables only)"),
                    new ListBagView<BasicSlotView>(store.Hotbar))
            ).Gap(50);

        }

        VisualElement Gold() {
            var label = Label("player-gold-label", "");
            var icon = Div("player-gold-icon");
            var el = Div("player-gold", icon, label);
            el.Observe(Store.Instance.Gold, value => label.text = value.ToString());
            return el;
        }

    }

}
