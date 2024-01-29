using System;
using System.Collections.Generic;

namespace Core.Events 
{
    public enum EventType 
    {
        LangChanged,
        UncleSamsParty
    }
    
    // TODO: Replace with Main MessageBroker
    public static class EventManager 
    {
        private static readonly Dictionary<EventType, List<Action<object[]>>> _handlers = new();
        
        public static void AddHandler(EventType type, Action<object[]> handler)
        {
            if (_handlers.TryGetValue(type, out var container)) 
            {
                container.Add(handler);
            }
            else {
                var handlers = new List<Action<object[]>> { handler };
                _handlers.Add(type, handlers);
            }
        }
        
        public static void RemoveHandler(EventType type, Action<object[]> handler) 
        {
            if (_handlers.TryGetValue(type, out var container)) 
            {
                container.Remove(handler);
            }
        }

        public static void RaiseEvent(EventType type, params object[] args) 
        {
            if (_handlers.TryGetValue(type, out var container)) 
            {
                foreach (var handler in container) 
                {
                    handler(args);
                }
            }
        }
    }
}