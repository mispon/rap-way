using UI.Enums;

namespace MessageBroker.Messages.UI
{
    public struct WindowControlMessage
    {
        public WindowControlMessage(WindowType type, object ctx = null)
        {
            Type    = type;
            Context = ctx;
        }
        
        public readonly WindowType Type;
        public readonly object     Context;
    }
}
