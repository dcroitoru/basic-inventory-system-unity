using UnityEngine.UIElements;
using GDS.Core;
using GDS.Basic.Views;
using GDS.Core.Views;

namespace GDS.Basic {
    public class StashWindow : VisualElement {
        public StashWindow(Stash bag) {
            this.Add("window",
                Comps.CloseButton(bag),
                Dom.Title("Stash"),
                new ListBagView<BasicSlotView>(bag)
            );
        }
    }
}
