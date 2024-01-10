using UniRx;
using Utils;

namespace Game.UI.Messages
{
    public class UIMessageBroker : Singleton<UIMessageBroker>
    {
        public MessageBroker MessageBroker { get; } = new();
    }
}