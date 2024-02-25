namespace MessageBroker.Messages.Player.State
{
    public struct ChangeFansMessage
    {
        public int Amount;
    }
    
    public struct FansChangedMessage
    {
        public int OldVal;
        public int NewVal;
    }
}