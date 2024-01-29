using System;
using Core;

namespace UI.MessageBroker
{
    public class UIMessageBroker : Singleton<UIMessageBroker>
    {
        private readonly UniRx.MessageBroker _broker = new();

        public void Publish<T>(T message)
        {
            _broker.Publish(message);
        }

        public IObservable<T> Receive<T>()
        {
            return _broker.Receive<T>();
        }
    }
}