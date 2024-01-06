using Enums;

namespace Messages.State 
{
    public struct FullStateRequest {}

    public struct FullStateResponse
    {
        public string NickName;
        public Gender Gender;
        public int Money;
        public int Fans;
        public int Hype;
        public int Exp;
    }
}