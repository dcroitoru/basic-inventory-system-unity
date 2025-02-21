using System;
using System.Collections.Generic;

namespace GDS.Core.Events {
    /// <summary>
    /// EventBus defines a channel on which events can be published and it allows subscribers be notified when 
    /// an event of that type has been published on the channel
    /// </summary>
    public class EventBus {
        public static readonly EventBus GlobalBus = new();

        public Dictionary<Type, List<Action<CustomEvent>>> subscribersByType = new();

        public void Subscribe<T>(Action<CustomEvent> eventHandler) where T : CustomEvent {
            var type = typeof(T);
            if (!subscribersByType.TryGetValue(type, out var subscribers)) subscribersByType.Add(type, new());
            subscribersByType[type].Add(eventHandler);
        }

        public void Unsubscribe<T>(Action<CustomEvent> eventHandler) where T : CustomEvent {
            var type = typeof(T);
            if (subscribersByType.TryGetValue(type, out var subscribers)) subscribers.Remove(eventHandler);
        }

        public void Publish(CustomEvent CustomEvent) {
            Type t = CustomEvent.GetType();
            if (subscribersByType.TryGetValue(t, out var subscribers)) subscribers.ForEach(s => s.Invoke(CustomEvent));
        }


    }
}