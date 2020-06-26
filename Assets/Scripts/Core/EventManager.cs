using System;
using System.Collections.Generic;

namespace Core 
{
    /// <summary>
    /// Типы событий
    /// </summary>
    public enum EventType 
    {
        LangChanged
    }

    /// <summary>
    /// Реализация менеджера игровых эвентов 
    /// </summary>
    public static class EventManager 
    {
        private static readonly Dictionary<EventType, List<Action<object[]>>> _handlers = 
            new Dictionary<EventType, List<Action<object[]>>>();

        /// <summary>
        /// Добавляет слушателя событий переданного типа
        /// </summary>
        public static void AddHandler(EventType type, Action<object[]> handler)
        {
            if (_handlers.ContainsKey(type)) 
            {
                _handlers[type].Add(handler);
            }
            else {
                var handlers = new List<Action<object[]>> { handler };
                _handlers.Add(type, handlers);
            }
        }

        /// <summary>
        /// Удаляет слушателя событий переданного типа
        /// </summary>
        public static void RemoveHandler(EventType type, Action<object[]> handler) 
        {
            if (_handlers.ContainsKey(type)) 
            {
                _handlers[type].Remove(handler);
            }
        }

        /// <summary>
        /// Отправляет эвент всем слушателям
        /// </summary>
        public static void RaiseEvent(EventType type, params object[] args) 
        {
            if (_handlers.ContainsKey(type)) 
            {
                foreach (var handler in _handlers[type]) 
                {
                    handler(args);
                }
            }
        }
    }
}