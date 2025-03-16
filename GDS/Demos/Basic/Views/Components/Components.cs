
using GDS.Core;
using GDS.Core.Events;
using UnityEngine.UIElements;

namespace GDS.Basic.Views {
    public static class Components {
        public static VisualElement CloseButton(Bag bag) => new Button(() => Store.Bus.Publish(new CloseWindowEvent(bag))).WithClass("close-button");
    }
}