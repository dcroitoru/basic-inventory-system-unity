using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using static GDS.Core.Dom;
using static GDS.Basic.Views.Comps;

namespace GDS.Basic {

    public class InventoryView : VisualElement {
        public InventoryView() {
            var store = Store.Instance;

            this.Add("window",
                CloseButton(store.Main),
                Div(
                    Title("Equipment (Equipment only)"),
                    new EquipmentView(store.Equipment)),
                Div(
                    Title("Inventory (Unrestricted)"),
                    new ListBagView<BasicSlotView>(store.Main)),
                Div(
                    Title("Hotbar (Consumables only)"),
                    new ListBagView<SlotView>(store.Hotbar))
            ).Gap(50);

        }

    }

}
