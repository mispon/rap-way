using UniRx;
using Utils;

namespace Game.UI.Messages
{
    public class UIMessageBroker : Singleton<UIMessageBroker>
    {
        public UniRx.MessageBroker MessageBroker { get; } = new();
    }
}