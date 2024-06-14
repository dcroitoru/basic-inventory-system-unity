using System;
using GDS;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDS {
    abstract public class Component<T> : VisualElement {

        protected T _data;
        public T Data {
            get => _data;
            set {
                if (_data == null || !_data.Equals(value)) {
                    _data = value;
                    Render(_data);
                }
            }
        }

        virtual public void Render(T data) { }


    }
}