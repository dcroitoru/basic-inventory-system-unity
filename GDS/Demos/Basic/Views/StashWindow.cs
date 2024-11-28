using UnityEngine.UIElements;
using GDS.Core;
using GDS.Basic.Views;

namespace GDS.Basic {
    public class StashWindow : VisualElement {
        public StashWindow(Stash bag) {
            this.Add("inventory-window",
                Dom.Title("Stash"),
                new BasicListBagView<ListBag>(bag)
            ).WithWindowBehavior(bag.Id, Store.Instance.sideWindowId, Store.Bus);
        }
    }
}
