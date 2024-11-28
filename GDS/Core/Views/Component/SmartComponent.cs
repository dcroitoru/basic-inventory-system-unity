using System;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS.Core.Views {
    /// <summary>
    /// A "smart" component that watches its data for changes and re-renders itself.
    /// Has some basic lifecycle methods (init, destroy)
    /// </summary>
    abstract public class SmartComponent<T> : VisualElement where T : INotifyChange {
        public SmartComponent(T data) {
            _data = data;
            void render(object o) => Render((T)o);

            // Subscribe to data changes and fire off the lifecycle (init, and initial render).
            // This needs to happen in AttachToPanelEvent callback.
            RegisterCallback<AttachToPanelEvent>((e) => {
                // Util.Log("attached to panel, should call render".pink(), this);
                data.OnChange += render;
                OnInit();
                Render(_data);
            });
            // Unsubscribe from data changes and destroy the component
            RegisterCallback<DetachFromPanelEvent>((e) => {
                // Util.Log("detached from panel".pink(), this);
                data.OnChange -= render;
                OnDestroy();
            });
        }

        readonly T _data;
        public T Data { get => _data; }
        public virtual void Render(T data) { }
        public virtual void OnInit() { }
        public virtual void OnDestroy() { }

    }
}