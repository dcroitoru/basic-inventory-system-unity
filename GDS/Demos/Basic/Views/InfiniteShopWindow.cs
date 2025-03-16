using UnityEngine.UIElements;
using GDS.Basic.Events;
using GDS.Basic.Views;
using GDS.Core;
using GDS.Core.Events;
using GDS.Core.Views;
using static GDS.Core.Dom;

namespace GDS.Basic {

    public class InfiniteShopWindow : VisualElement {

        public InfiniteShopWindow(Vendor bag) {

            void BuyRandom() => Store.Bus.Publish(new BuyRandomItem(bag));

            void SellItem(Item item) => Store.Bus.Publish(new SellItemEvent(item));

            this.Add("window vendor",
                Title("Shop (Infinite resources)"),
                new ListBagView<SlotView<BasicItemView>>(bag),
                Button("Buy Random", BuyRandom),
                new DropTargetView(Store.Instance.DraggedItem, SellItem),
                Components.CloseButton(bag)
            );
        }
    }
}
