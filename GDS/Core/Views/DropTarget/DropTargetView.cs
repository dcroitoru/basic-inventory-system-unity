using System;
using UnityEngine.UIElements;

namespace GDS.Core {
    public class DropTargetView : VisualElement {
        public DropTargetView(Observable<Item> dragged, Action<Item> callback) {
            this.WithClass("drop-target").Hide();
            this.Observe(dragged, item => {
                this.ToggleClass("drop-target-visible", item is not NoItem);
                this.SetVisible(item is not NoItem);
            });
            RegisterCallback<PointerUpEvent>(e => {
                if (e.button != 0) return;
                if (dragged.Value is NoItem) return;
                callback(dragged.Value);
            });

        }
    }
}
