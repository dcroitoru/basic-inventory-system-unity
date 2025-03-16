using System;
using System.Linq;

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
                    BaseSlotView t when draggedItem.Value is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data, e.modifiers),
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

            void OnMouseDown(PointerDownEvent e) {
                if (e.button != 0) return;
                if (draggedItem.Value is not NoItem) return;
                mouseDownPos = e.position;
                target = e.target as VisualElement;
                dragStarted = true;
            }

            void OnMouseMove(PointerMoveEvent e) {
                if (dragStarted == false) return;
                if (draggedItem.Value is not NoItem) return;
                if (Math.Abs(mouseDownPos.x - e.position.x) < MinDragDistance && Math.Abs(mouseDownPos.y - e.position.y) < MinDragDistance) return;

                CustomEvent evt = target switch {
                    BaseSlotView t when draggedItem.Value is NoItem => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    _ => new NoEvent()
                };

                dragStarted = false;
                bus.Publish(evt);
            }


            void OnMouseUp(PointerUpEvent e) {
                if (e.button != 0) return;
                dragStarted = false;

                CustomEvent evt = e.target switch {
                    BaseSlotView t when draggedItem.Value is NoItem && target == t => new PickItemEvent(t.Bag, t.Data.Item, t.Data, e.modifiers),
                    BaseSlotView t when draggedItem.Value is not NoItem => new PlaceItemEvent(t.Bag, draggedItem.Value, t.Data, e.modifiers),

                    _ => new NoEvent()
                };
                bus.Publish(evt);
            }


            element.RegisterCallback<PointerDownEvent>(OnMouseDown);
            element.RegisterCallback<PointerUpEvent>(OnMouseUp);
            element.RegisterCallback<PointerMoveEvent>(OnMouseMove);
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
        public static VisualElement WithItemTooltipBehavior<T>(this VisualElement root) where T : Component<BagItem> => WithItemTooltipBehavior<T>(root, DefaultPosition, () => { });
        public static VisualElement WithItemTooltipBehavior<T>(this VisualElement root, Func<Rect, Rect, Rect, (float, float)> positionStrategy, Action callback) where T : Component<BagItem> {

            T tooltip = Activator.CreateInstance<T>();
            var pollingInterval = 50;
            var container = tooltip.IgnorePickAll();
            var visible = false;
            container.Hide();
            root.Add(container);
            Item lastItem = Item.NoItem;
            Rect lastItemBounds = new();

            void ConditionalHide() { if (visible == true) { visible = false; lastItem = Item.NoItem; container.Hide(); } }
            void ConditionalShow() { if (visible == false) { visible = true; container.Show(); } }

            Rect rootBounds = root.worldBound;
            Vector2 screenPos;
            VisualElement picked;
            VisualElement lastPicked = new();
            float arw;
            float arh;
            bool skipFlag = true;

            tooltip.RegisterCallback<GeometryChangedEvent>(e => {
                if (container.worldBound.width == 0) return;
                if (skipFlag) return;

                PositionTooltip();
                skipFlag = true;
            });

            void PositionTooltip() {
                var (left, top) = positionStrategy(lastItemBounds, container.worldBound, rootBounds);
                container.style.left = left;
                container.style.top = top;
            }

            // Note: Code is a little ugly because it's a high frequency function and it needs to be somewhat optimal
            root.schedule.Execute(() => {
                arw = root.worldBound.width / Screen.width;
                arh = root.worldBound.height / Screen.height;
                screenPos = Input.mousePosition;
                screenPos.x *= arw;
                screenPos.y = (Screen.height - screenPos.y) * arh;
                // TODO: optimize this - add a caching mechanism and return early if picked element and bounds haven't changed                
                picked = root.panel.Pick(screenPos);

                // TODO: refactor this - should not create ANY vars in this function (guaranteed garbage)
                (Bag bag, Item item, Rect bounds) = picked switch {
                    ISlotView s => (s.Bag, s.Data.Item, picked.worldBound),
                    _ => (Bag.NoBag, Item.NoItem, new Rect())
                };

                if (item is NoItem) { ConditionalHide(); return; }
                if (item.Id == lastItem.Id) return;
                lastItem = item;
                lastItemBounds = bounds;
                // Set data
                tooltip.Data = new(bag, item);
                PositionTooltip();
                skipFlag = false;

                ConditionalShow();

                callback();

            }).Every(pollingInterval);

            return root;
        }


        /// <summary>
        /// <para>Try to position the tooltip next to the element, first on Top, then Left, then Right.</para>
        /// <para>Will stick </para>
        /// </summary>
        /// <returns>The new top-left</returns>
        public static (float left, float top) DefaultPosition(Rect itemBounds, Rect tooltipBounds, Rect rootBounds) {
            float left, top;
            top = itemBounds.yMin - tooltipBounds.height;
            left = itemBounds.center.x - tooltipBounds.width / 2;

            // TODO: technically this could be a switch...
            if (top < 0) {
                top = 0;
                left = itemBounds.xMin - tooltipBounds.width;
                if (left < 0) {
                    left = itemBounds.xMax;
                }
            } else {
                if (left < 0) {
                    left = 0;
                }
                if (left + tooltipBounds.width > rootBounds.width) {
                    left = rootBounds.width - tooltipBounds.width;
                }
            }

            return (left, top);
        }

        /// <summary>
        /// Adds a layer that will serve as a drop target. Requires USS classes.
        /// </summary>        
        public static T WithDropTargetBehavior<T>(this T element, Observable<Item> dragged, Action<Item> callback) where T : VisualElement {
            element.Add(new DropTargetView(dragged, callback));
            return element;
        }

        public static T WithItemHoverEvent<T>(this T element, Action<Item> callback) where T : VisualElement {
            EventCallback<MouseOverEvent> onMouseOver = e => {

            };

            element.RegisterCallback(onMouseOver);
            return element;
        }
    }


}