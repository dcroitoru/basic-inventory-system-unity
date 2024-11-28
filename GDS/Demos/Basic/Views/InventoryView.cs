using System;
using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Views;
using GDS.Sample;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Core.Dom;
namespace GDS.Basic {

    public class InventoryView : VisualElement {

        public InventoryView() {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/BasicInventory"));
            var store = Store.Instance;
            store.IsInventoryOpen.OnChange += val => {
                this.SetVisible(val);
            };

            this.Add("inventory-window",
                Div(Title("Equipment (Equipment only)"), new EquipmentView(store.Equipment)),
                Div(Title("Inventory (Unrestricted)"), new BasicListBagView<ListBag>(store.Inventory)),
                Div(Title("Hotbar (Consumables only)"), new ListBagView<ListBag>(store.Hotbar))
            ).Gap(50);
        }

    }

}
