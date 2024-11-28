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
            OnChange.Invoke(Value);
        }
    }

    /// <summary>
    /// An interface that allows subscribers be notified when a change occured.
    /// It is similar to Observable but it is not generic and doesn't have an provide a 
    /// public method to set a new value.
    /// </summary>
    public interface INotifyChange {
        public event Action<object> OnChange;
        public void Notify();
    }


}