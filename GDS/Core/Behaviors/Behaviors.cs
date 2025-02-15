using System;

using GDS.Core.Events;
using GDS.Core.Views;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core {
    public static class Behaviors {

        public static VisualElement WithRestrictedSlotBehavior(this VisualElement element, Observable<Item> draggedItem) {
            var NoDiv = Dom.Div();
            var lastHoveredItem = NoDiv;

            EventCallback<PointerOverEvent> OnMouseOver = (e) => {
                if (e.target is not BaseSlotView) return;
                lastHoveredItem = e.target as VisualElement;
                if (draggedItem.Value is NoItem) return;
                bool legalAction = e.target switch {
                    BaseSlotView view => view.Bag.Accepts(draggedItem.Value) && view.Data.Accepts(draggedItem.Value),
                    _ => false
                };

                string className = legalAction ? "legal-action" : "illegal-action";
                lastHoveredItem.WithClass(className);
            };

            EventCallback<PointerOutEvent> OnMouseOut = (e) => {
                if (draggedItem.Value is NoItem) return;
                if (e.target is not BaseSlotView) return;
                if (lastHoveredItem == null) return;
                lastHoveredItem.WithoutClass("illegal-action legal-action");
                lastHoveredItem = NoDiv;
            };

            element.Observe(draggedItem, item => {
                if (item is NoItem) lastHoveredItem.WithoutClass("illegal-action legal-action");
            });

            element.RegisterCallback(OnMouseOver);
            element.RegisterCallback(OnMouseOut);

            return element;
        }

        public static VisualElement WithClickToPickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus) {
            EventCallback<PointerDownEvent> OnMouseDown = (e) => {
                CustomEvent evt = e.target switch {
                    BaseSlotView t when draggedItem.Value is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when draggedItem.Value is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
            };

            element.RegisterCallback(OnMouseDown);

            return element;
        }

        public static VisualElement WithDragToPickBehavior(this VisualElement element, Observable<Item> draggedItem, EventBus bus, int MinDragDistance = 32) {
            var dragStarted = false;
            var mouseDownPos = new Vector2(-1, -1);
            var target = new VisualElement();

            EventCallback<PointerDownEvent> OnMouseDown = (e) => {
                if (e.button != 0) return;
                if (draggedItem.Value is not NoItem) return;
                mouseDownPos = e.position;
                dragStarted = true;
                target = e.target as VisualElement;
            };

            EventCallback<PointerMoveEvent> OnMouseMove = (e) => {
                if (dragStarted == false) return;
                if (draggedItem.Value is not NoItem) return;
                if (Math.Abs(mouseDownPos.x - e.position.x) < MinDragDistance && Math.Abs(mouseDownPos.y - e.position.y) < MinDragDistance) return;

                CustomEvent evt = target switch {
                    BaseSlotView t when draggedItem.Value is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
                dragStarted = false;
            };


            EventCallback<PointerUpEvent> OnMouseUp = (e) => {
                if (e.button != 0) return;
                dragStarted = false;
                CustomEvent evt = e.target switch {
                    BaseSlotView t when draggedItem.Value is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when draggedItem.Value is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data),
                    _ => new NoEvent()
                };
                bus.Publish(evt);
            };

            element.RegisterCallback(OnMouseDown);
            element.RegisterCallback(OnMouseUp);
            element.RegisterCallback(OnMouseMove);
            return element;
        }

        /// <summary>
        /// Shows the dragged item at cursor position as it is being dragged. 
        /// Should be acontainerached to the root element (dragged element moves in screen space).
        /// Requires `ghost-item` uss class (absolute pos, % translate).
        /// </summary>
        public static VisualElement WithGhostItemBehavior<TGhostItemView>(this VisualElement root, Observable<Item> draggedItem) where TGhostItemView : Component<Item> {

            TGhostItemView itemView = Activator.CreateInstance<TGhostItemView>();
            var ghost = Dom.Div("ghost-item", itemView).IgnorePickAll();
            root.Add(ghost);

            EventCallback<PointerMoveEvent> OnPointerMove = e => {
                ghost.style.left = e.localPosition.x;
                ghost.style.top = e.localPosition.y;
            };

            root.Observe(draggedItem, item => {
                if (item is NoItem) { ghost.Hide(); return; }

                itemView.Data = item;
                ghost.Show();
            });

            root.RegisterCallback(OnPointerMove);
            return root;
        }

        /// <summary>
        /// Shows an item tooltip (provided as a renderer T) when the cursor is over a non empty slot
        /// </summary>
        /// <returns></returns>
        public static VisualElement WithItemTooltipBehavior<T>(this VisualElement root) where T : Component<Item> {

            T tooltip = Activator.CreateInstance<T>();
            var pollingInterval = 50;
            var container = tooltip.IgnorePickAll();
            var visible = false;
            container.Hide();
            root.Add(container);

            Vector2 screenPos;
            VisualElement picked;
            float arw;
            float arh;
            // Note: 
            // Code is a little ugly because it's a high frequency function and it needs to be somewhat optimal
            root.schedule.Execute(() => {
                arw = root.worldBound.width / Screen.width;
                arh = root.worldBound.height / Screen.height;
                screenPos = Input.mousePosition;
                screenPos.x *= arw;
                screenPos.y = (Screen.height - screenPos.y) * arh;
                picked = root.panel.Pick(screenPos);

                // TODO: add a caching mechanism and return early if slot hasn't changed
                if (picked is not BaseSlotView s) {
                    if (visible == true) {
                        visible = false;
                        container.Hide();
                    }
                    return;
                }

                if (s.Data.IsEmpty()) {
                    visible = false;
                    container.Hide();
                    return;
                }

                // Set data
                tooltip.Data = s.Data.Item;

                // Set position
                // TODO: bring back old behavior where tooltip self positions to fir inside screen bounds
                container.style.left = picked.worldBound.center.x;
                container.style.top = picked.worldBound.yMin;

                if (visible == false) {
                    visible = true;
                    container.Show();
                }
            }).Every(pollingInterval);

            return root;
        }


    }


}