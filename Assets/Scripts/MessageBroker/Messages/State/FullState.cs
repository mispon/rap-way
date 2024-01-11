using Enums;

namespace MessageBroker.Messages.State 
{
    public struct FullStateRequest {}

    public struct FullStateResponse
    {
        public string NickName;
        public Gender Gender;
        public int Money;
        public int Donate;
        public int Fans;
        public int Hype;
        public int Exp;
    }
}