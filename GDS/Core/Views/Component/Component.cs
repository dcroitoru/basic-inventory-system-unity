using UnityEngine.UIElements;

namespace GDS.Core.Views {
    /// <summary>
    /// A "dumb" component that re-renders itself when its `data` changes (from outside)
    /// Data (T) needs to be a value type for equality check to work
    /// </summary>
    abstract public class Component<T> : VisualElement {

        bool _rendered = false;
        T _data;
        public T Data {
            get => _data;
            set {
                // Return early if `data` hasn't changed
                if (_rendered == true && _data.Equals(value)) return;
                _rendered = true;
                _data = value;
                Render(_data);
            }
        }

        public virtual void Render(T data) { }

    }
}