using System;
using System.Collections.Generic;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;
using static GDS.Util;
using static GDS.Factory;

namespace GDS {

    public class GhostItemView : VisualElement {
        public GhostItemView(VisualElement root, Observable<Item> draggedItemStream, EventBus bus) {
            this.root = root;
            this.Div("ghost-item", image.WithClass("item-image")).WithoutPointerEvents();

            this.bus = bus;
            draggedItem = draggedItemStream.Value;
            draggedItemStream.OnNext += Render;
            Render(draggedItem);

            root.RegisterCallback<ClickEvent>(onRootClick);
            Action<CustomEvent> action = e => onSlotMouseOver(e as SlotMouseOverEvent);
            Global.GlobalBus.Subscribe<SlotMouseOverEvent>(action);

        }

        int CellSize = 64;
        VisualElement root;

        VisualElement image = new();
        SlotView dropTarget;
        Item draggedItem = NoItem;
        EventBus bus;
        public bool UseItemSize = false;


        public void Render(Item data) {
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
            var itemSize = UseItemSize ? data.Size : new Size(1, 1);
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            style.display = DisplayStyle.Flex;
            image.style.backgroundImage = new StyleBackground(data.Image());
            image.SetSize(itemSize, CellSize);
            image.Translate(itemSize, -CellSize / 2);
        }

        void SetPosition(Vector3 pos) {
            style.left = pos.x;
            style.top = pos.y;
        }


        void OnPointerMove(PointerMoveEvent e) => SetPosition(e.localPosition);

        void onRootClick(ClickEvent e) {
            var target = e.target;
            CustomEvent evt = (target, draggedItem) switch {
                (SlotView, Item i) when i == NoItem => new PickItemEvent((target as SlotView).Bag, (target as SlotView).Data.Item),
                (SlotView, Item i) when i != NoItem => new PlaceItemEvent((target as SlotView).Bag, draggedItem, (target as SlotView).Data),
                _ => new NoEvent()
            };

            bus.Publish(evt);
            SetPosition(e.localPosition);
        }

        void onSlotMouseOver(SlotMouseOverEvent e) {
            if (draggedItem == NoItem) return;
            dropTarget?.ClearDropTargetVisual();
            dropTarget = e.SlotView;
            var slotType = dropTarget.Data.Type;
            dropTarget.SetDropTargetVisual(Fn.CanPlace(slotType, draggedItem));
        }
    }
}