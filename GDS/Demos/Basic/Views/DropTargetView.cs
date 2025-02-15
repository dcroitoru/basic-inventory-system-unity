using UnityEngine.UIElements;
using GDS.Core;
using GDS.Basic.Views;
using GDS.Core.Views;

namespace GDS.Basic {
    public class DropTargetView : VisualElement {
        public DropTargetView() {
            this.WithClass("background");
            this.Observe(Store.Instance.DraggedItem, item => this.ToggleClass("background-drop-target", item is not NoItem));
        }
    }
}
