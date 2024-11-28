using System;
using GDS.Core.Events;
using GDS.Core.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core {
    public static class Behaviors {

        public static VisualElement WithWindowBehavior(this VisualElement element, string windowId, Observable<string> activeWindowId, EventBus bus) {
            activeWindowId.OnChange += id => element.SetVisible(id == windowId);
            return element.SetVisible(false);
        }

        public static VisualElement WithTooltipBehavior(this VisualElement element, Tooltip tooltip) {
            EventCallback<ClickEvent> OnClick = (e) => {
                tooltip.Hide();
            };

            EventCallback<MouseOverEvent> OnMouseOver = (e) => {
                if (e.target is not BaseSlotView && e.target is not BaseSetSlotView) return;
                (Item item, Rect bounds) = e.target switch {
                    BaseSlotView x => (x.Data.Item, x.worldBound),
                    BaseSetSlotView x => (x.Data.Item, x.worldBound),
                    _ => (Item.NoItem, new Rect())
                };
                if (item is NoItem) return;
                tooltip.style.opacity = 0;
                tooltip.Show();
                tooltip.Render(item);
                tooltip.schedule.Execute(() => {
                    tooltip.SetPosition(bounds, element.worldBound);
                    tooltip.style.opacity = 1;
                }).ExecuteLater(15);

            };

            EventCallback<MouseOutEvent> OnMouseOut = (e) => {
                if (e.target is not BaseSlotView && e.target is not BaseSetSlotView) return;
                tooltip.Hide();
                tooltip.style.visibility = Visibility.Hidden;
            };

            element.RegisterCallback(OnMouseOver);
            element.RegisterCallback(OnMouseOut);
            element.RegisterCallback(OnClick);

            return element;
        }

        public static VisualElement WithDefaultHoverBehavior(this VisualElement element, Observable<Item> draggedItem) {

            var lastHoveredItem = Dom.Div();

            EventCallback<PointerOverEvent> OnMouseOver = (e) => {
                if (e.target is not BaseSlotView && e.target is not BaseSetSlotView) return;
                lastHoveredItem = e.target as VisualElement;
                if (draggedItem.Value is NoItem) return;
                bool legalAction = e.target switch {
                    BaseSetSlotView view => view.Bag.Accepts(draggedItem.Value) && view.Data.Accepts(draggedItem.Value),
                    BaseSlotView view => view.Bag.Accepts(draggedItem.Value) && view.Data.Accepts(draggedItem.Value),
                    _ => false
                };

                string className = legalAction ? "legal-action" : "illegal-action";
                lastHoveredItem.WithClass(className);
            };

            EventCallback<PointerOutEvent> OnMouseOut = (e) => {
                if (draggedItem.Value is NoItem) return;
                if (e.target is not BaseSlotView && e.target is not BaseSetSlotView) return;
                if (lastHoveredItem == null) return;
                lastHoveredItem.WithoutClass("illegal-action legal-action");
                lastHoveredItem = null;
            };

            draggedItem.OnChange += (item) => {
                if (item is NoItem) {
                    lastHoveredItem?.WithoutClass("illegal-action legal-action");
                    return;
                }
            };

            element.RegisterCallback(OnMouseOver);
            element.RegisterCallback(OnMouseOut);


            return element;
        }

        public static VisualElement WithDefaultItemClickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) {
            EventCallback<ClickEvent> OnClick = (e) => {
                var dragged = draggedItem.Value;
                CustomEvent evt = e.target switch {
                    BaseSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    BaseSetSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSetSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
            };

            element.RegisterCallback(OnClick);
            return element;
        }

        public static VisualElement WithDefaultPickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) => WithDiabloItemPickBehavior(element, draggedItem, bus);

        public static VisualElement WithDiabloItemPickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) {
            EventCallback<PointerDownEvent> OnMouseDown = (e) => {
                var dragged = draggedItem.Value;
                CustomEvent evt = e.target switch {
                    BaseSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    BaseSetSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSetSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
            };

            element.RegisterCallback(OnMouseDown);

            return element;
        }

        public static VisualElement WithPOEItemPickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) {
            Vector2 mouseDownPos = new Vector3(-1, -1);
            bool dragStarted = false;
            VisualElement target = new VisualElement();

            EventCallback<PointerDownEvent> OnMouseDown = (e) => {
                var dragged = draggedItem.Value;
                if (dragged is not NoItem) return;
                mouseDownPos = e.position;
                dragStarted = true;
                target = e.target as VisualElement;
            };

            EventCallback<PointerMoveEvent> OnMouseMove = (e) => {
                if (dragStarted == false) return;
                var dragged = draggedItem.Value;
                if (dragged is not NoItem) return;
                if (Math.Abs(mouseDownPos.x - e.position.x) < 32 && Math.Abs(mouseDownPos.y - e.position.y) < 32) return;

                CustomEvent evt = target switch {
                    BaseSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSetSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
                dragStarted = false;
            };


            EventCallback<PointerUpEvent> OnMouseUp = (e) => {
                dragStarted = false;
                var dragged = draggedItem.Value;
                CustomEvent evt = e.target switch {
                    BaseSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    BaseSetSlotView t when dragged is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSetSlotView t when dragged is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
            };

            element.RegisterCallback(OnMouseDown);
            element.RegisterCallback(OnMouseUp);
            element.RegisterCallback(OnMouseMove);
            return element;
        }

        public static VisualElement WithDropItemBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) {
            EventCallback<PointerUpEvent> OnMouseUp = (e) => {
                if (e.target == element && draggedItem.Value is not NoItem) {
                    Debug.Log("should dispatch drop event");
                    bus.Publish(new DropDraggedItemEvent());
                }
            };
            element.RegisterCallback(OnMouseUp);
            return element;
        }
    }
}