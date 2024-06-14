using System;
using System.Collections.Generic;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Util;
using static GDS.Factory;

namespace GDS {

    public class BasicGhostItemView : VisualElement {
        public BasicGhostItemView(VisualElement root, EventBus bus) {
            this.root = root;
            this.bus = bus;
            this.Div("ghost-item", image.WithClass("item-image"));
            pickingMode = PickingMode.Ignore;
            image.pickingMode = PickingMode.Ignore;
            var store = Global.BasicInventoryStore;
            store.draggedItem.OnNext += Render;
            Render(store.draggedItem.Value);

            root.RegisterCallback<ClickEvent>(onRootClick);
            Action<CustomEvent> action = e => onSlotHover(e as SlotHoverEvent);
            Global.GlobalBus.Subscribe<SlotHoverEvent>(action);
            RegisterCallback<DetachFromPanelEvent>((e) => {
                Global.GlobalBus.Unsubscribe<SlotHoverEvent>(action);
                store.draggedItem.OnNext -= Render;
            });
        }

        // int CellSize = 64;
        EventBus bus;
        VisualElement root;

        VisualElement image = new();
        SlotView dropTarget;
        Item draggedItem;

        public void Render(Item data) {
            Util.Log("should render ghost icon".blue(), data.ToString().green());
            if (data == NoItem) Hide();
            else Show(data);
            draggedItem = data;
        }

        void Hide() {
            style.display = DisplayStyle.None;
            root.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            dropTarget?.ClearDropTargetVisual();
        }

        void Show(Item data) {
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            style.display = DisplayStyle.Flex;
            image.style.backgroundImage = new StyleBackground(data.Image());
        }

        void SetPosition(Vector3 pos) {
            style.left = pos.x;
            style.top = pos.y;
        }


        void OnPointerMove(PointerMoveEvent e) => SetPosition(e.localPosition);

        void onRootClick(ClickEvent e) {
            Util.Log("onRootClick".orange(), e.target.ToString().yellow());
            var draggedItem = Global.BasicInventoryStore.draggedItem.Value;
            var target = e.target;
            CustomEvent evt = (target, draggedItem) switch {
                (SlotView, Item i) when i == NoItem => new PickItemEvent((target as SlotView).Bag, (target as SlotView).Data.Item),
                (SlotView, Item i) when i != NoItem => new PlaceItemEvent((target as SlotView).Bag, draggedItem, (target as SlotView).Data),
                _ => new NoEvent()
            };

            bus.Publish(evt);
            SetPosition(e.localPosition);
        }

        void onSlotHover(SlotHoverEvent e) {
            if (draggedItem == NoItem) return;
            dropTarget?.ClearDropTargetVisual();
            dropTarget = e.SlotView;
            var slotType = dropTarget.Data.Type;
            dropTarget.SetDropTargetVisual(Fn.CanPlace(slotType, draggedItem));
        }
    }
}