using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace GDS.Core.Views {

    public class GhostItemView : GhostItemView<ItemView> { public GhostItemView(VisualElement root, Observable<Item> draggedItem) : base(root, draggedItem) { } }

    public class GhostItemView<T> : VisualElement where T : Component<Item> {
        public GhostItemView(VisualElement root, Observable<Item> draggedItem) {

            this.Div("ghost-item",
                itemView
            ).WithoutPointerEventsInAll();


            Render(draggedItem.Value);

            draggedItem.OnChange += Render;
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        T itemView = Activator.CreateInstance<T>();
        public int CellSize = 64;
        public bool UseItemSize = false;

        public void Render(Item data) {
            if (data is NoItem) {
                this.Hide();
                return;
            }

            var itemSize = UseItemSize ? data.ItemBase.Size : new Size(1, 1);
            itemView.Data = data;
            itemView.SetSize(itemSize, CellSize);
            this.Translate(itemSize, -CellSize / 2);
            this.Show();
        }

        void SetPosition(Vector3 pos) {
            style.left = pos.x;
            style.top = pos.y;
        }

        void OnPointerMove(PointerMoveEvent e) {
            SetPosition(e.localPosition);
        }


    }
}