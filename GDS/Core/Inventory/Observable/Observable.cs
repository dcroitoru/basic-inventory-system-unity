using System;

namespace GDS.Core {

    /// <summary>
    /// A class that wraps a value and allows subscribers to watch for value changes.
    /// </summary>
    public class Observable<T> {
        public Observable(T initialValue) { Value = initialValue; }
        public T Value { get; private set; }
        public event Action<T> OnChange = (_) => { };
        public void SetValue(T value) {
            Value = value;
            Notify();
        }
        public void Notify() {
            OnChange.Invoke(Value);
        }
    }
}