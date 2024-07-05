using System;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS {
    abstract public class SmartComponent<T> : VisualElement where T : IObservable {
        public SmartComponent(T data) {
            _data = data;

            RegisterCallback<AttachToPanelEvent>((e) => {
                // Util.Log("attached to panel, should call render".pink(), this);
                OnInit();
                data.OnNext += _render;
                Render(_data);
            });
            RegisterCallback<DetachFromPanelEvent>((e) => {
                // Util.Log("detached from panel".pink(), this);
                OnDestroy();
                data.OnNext -= _render;
            });
        }

        protected T _data;
        public T Data { get => _data; }
        void _render(object o) {
            Render((T)o);
        }
        virtual public void Render(T data) { }
        virtual public void OnInit() { }
        virtual public void OnDestroy() { }

    }
}