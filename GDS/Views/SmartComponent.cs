using System;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS {
    abstract public class SmartComponent<T> : VisualElement {
        public SmartComponent(Observable<T> data) {
            _data = data.Value;

            RegisterCallback<AttachToPanelEvent>((e) => {
                // Util.Log("attached to panel, should call render".pink(), this);
                data.OnNext += Render;
                Render(_data);
            });
            RegisterCallback<DetachFromPanelEvent>((e) => {
                // Util.Log("detached from panel".pink(), this);
                data.OnNext -= Render;
            });
        }

        protected T _data;
        public T Data { get => _data; }
        virtual public void Render(T data) { }


    }
}