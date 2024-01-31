using UI.Enums;

namespace UI.MessageBroker.Messages
{
    public struct WindowControlMessage
    {
        public WindowType Type;

        public WindowControlMessage(WindowType type)
        {
            Type = type;
        }
    }
}
