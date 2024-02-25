using Enums;

namespace MessageBroker.Messages.Player
{
    public struct TeammateCooldownMessage
    {
        public TeammateType Type;
        public int Cooldown;
    }
}