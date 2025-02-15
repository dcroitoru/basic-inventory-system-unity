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
            var store = Store.Instance;
            // This declatative call creates the visual tree structure 
            // adds the `root-layer` uss class to current element
            this.Add("root-layer",
                // adds a view that renders the `Main Inventory`
                new ListBagView<SlotView>(store.MainInventory)
            )
            // adds a behavior that controls picking and placing items (default is pick and place on `MouseUp`)
            .WithGhostItemBehavior<ItemView>(Store.Instance.DraggedItem)
            .WithDragToPickBehavior(store.DraggedItem, store.Bus);

            // It is equivalent to this imperative block of code:
            // 
            // AddToClassList("root-layer");
            // Add(new ListBagView<SlotView>(store.MainInventory));
            // Add(new GhostItemService<ItemView>(this, store.DraggedItem));
            // this.WithClickToPickBehavior(store.DraggedItem, store.Bus);
        }
    }
}
