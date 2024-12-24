using Enums;

namespace MessageBroker.Messages.Player.State
{
    public struct FullStateRequest { }

    public struct FullStateResponse
    {
        public string NickName;
        public string RealName;
        public Gender Gender;
        public int    Money;
        public int    Donate;
        public int    Fans;
        public int    Hype;
        public int    Exp;
        public int    Level;
    }
}