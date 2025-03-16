using UnityEngine;
using UnityEngine.UIElements;
using GDS.Core;
using GDS.Core.Views;
using static GDS.Core.DomFactory;
namespace GDS.Minimal {
    /// <summary>
    /// The Root Layer is the top-most visual element
    /// </summary>
    public class RootLayer : VisualElement {
        public new class UxmlFactory : UxmlFactory<RootLayer> { }
        public RootLayer() {
            var store = Store.Instance;
            // This declatative call creates the visual tree structure 
            // adds the `root-layer` uss class to current element
            this.Add("root-layer",
                // adds a view that renders the `Main Inventory`
                ListBagView(store.MainInventory)
            )
            // adds a view that displays the currently dragged item
            .WithGhostItemBehavior<ItemView>(store.DraggedItem)
            // adds a behavior that controls picking and placing items (default is pick and place on `MouseUp`)
            .WithDragToPickBehavior(store.DraggedItem, store.Bus);
        }
    }
}
