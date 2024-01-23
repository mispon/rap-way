using Core;

namespace UI.MessageBroker
{
    public class UIMessageBroker : Singleton<UIMessageBroker>
    {
        public UniRx.MessageBroker MessageBroker { get; } = new();
    }
}