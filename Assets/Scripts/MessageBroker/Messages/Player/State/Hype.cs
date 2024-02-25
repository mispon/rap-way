namespace MessageBroker.Messages.Player.State
{
    public struct ChangeHypeMessage
    {
        public int Amount;
    }
    
    public struct HypeChangedMessage
    {
        public int OldVal;
        public int NewVal;
    }
}