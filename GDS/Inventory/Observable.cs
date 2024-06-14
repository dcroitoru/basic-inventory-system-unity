using System;

namespace GDS {
    public class Observable<T> {
        public Observable(T initialValue) { Value = initialValue; }
        public T Value { get; private set; }
        public event Action<T> OnNext = (_) => { };
        public void Next(T value) {
            Value = value;
            OnNext.Invoke(Value);
        }
    }
}