using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Dom;
namespace GDS {

    public class BasicInventoryWindow : VisualElement {

        public BasicInventoryWindow() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            var store = Global.BasicInventoryStore;
            store.isInventoryOpen.OnNext += SetVisible;

            this.
                Div("inventory-window",
                    Div("mb-10", Title("Equipment (equipment only)"), new BasicEquipmentView(store.equipment)),
                    Div("mb-10", Title("Inventory (unrestricted)"), new ListInventoryView<Inventory>(store.inventory)),
                    Div("mb-10", Title("Hotbar (consumables only)"), new ListInventoryView<Hotbar>(store.hotbar))
                );


        }

        void SetVisible(bool value) => style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
