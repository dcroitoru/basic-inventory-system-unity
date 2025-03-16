using UnityEngine.UIElements;
using GDS.Basic.Events;
using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Events;
using GDS.Core.Views;
using static GDS.Core.Dom;

namespace GDS.Basic {

    public class ShopWindow : VisualElement {

        public ShopWindow(Vendor bag) {

            void BuyRandom() => Store.Bus.Publish(new BuyRandomItem(bag));
            void Randomize() => Store.Bus.Publish(new RerollShop(bag));
            void SellItem(Item item) => Store.Bus.Publish(new SellItemEvent(item));

            this.Add("window vendor",
                Title("Shop"),
                new ListBagView<SlotView<BasicItemView>>(bag),
                Div("row", Button("Buy Random", BuyRandom), Button("Reroll Shop", Randomize)),
                new DropTargetView(Store.Instance.DraggedItem, SellItem),
                Components.CloseButton(bag)
            );
        }
    }
}
