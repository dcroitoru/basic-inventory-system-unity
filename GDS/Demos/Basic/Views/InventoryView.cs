using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Dom;
namespace GDS.Demos.Basic {

    public class InventoryView : VisualElement {

        public InventoryView() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            var store = Store.Instance;
            store.isInventoryOpen.OnNext += SetVisible;

            this.Div("inventory-window",
                Div("mb-10", Title("Equipment (Equipment only)"), new EquipmentView(store.equipment)),
                Div("mb-10", Title("Inventory (Unrestricted)"), new ListInventoryView<Inventory>(store.inventory)),
                Div("mb-10", Title("Hotbar (Consumables only)"), new ListInventoryView<Hotbar>(store.hotbar))
            );
        }

        void SetVisible(bool visible) => style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

    }
}
