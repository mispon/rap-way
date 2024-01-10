using System;
using UniRx;
using Utils;

namespace Game.UI.Messages
{
    public class UIMessageBroker : Singleton<UIMessageBroker>
    {
        [NonSerialized] public readonly MessageBroker MessageBroker = new();
    }
}