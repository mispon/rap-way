namespace MessageBroker.Messages.Player.State
{
    public struct ChangeExpMessage
    {
        public int Amount;
    }
    
    public struct ExpChangedMessage
    {
        public int OldVal;
        public int NewVal;
    }
}