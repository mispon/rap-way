using UI.Enums;

namespace UI.MessageBroker.Messages
{
    public struct WindowControlMessage
    {
        public WindowType Type;
        public object Meta;
    }
}
