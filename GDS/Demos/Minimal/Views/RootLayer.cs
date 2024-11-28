using UnityEngine;
using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
namespace GDS.Minimal {
    /// <summary>
    /// The Root Layer is the top-most visual element
    /// </summary>
    public class RootLayer : VisualElement {
        public RootLayer() {
            // Add the stylesheet from `Resources` folder
            styleSheets.Add(Resources.Load<StyleSheet>("Minimal/MinimalStyles"));
            var store = Store.Instance;
            // This one declatative call does the following:
            // - adds the `root-layer` uss class to current element
            // - adds a view that renders the `Main Inventory` (ListBagView)
            // - adds a view that displays the currently dragged item (GhostItemView)
            // - adds a behavior that controls picking and placing items (default is pick and place on `MouseUp`)
            this.Add("root-layer",
                new ListBagView<ListBag>(store.MainInventory),
                new GhostItemView(this, store.DraggedItem)
            ).WithDefaultPickBehavior(store.DraggedItem, store.Bus);
        }
    }
}
