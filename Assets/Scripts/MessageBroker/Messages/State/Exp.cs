namespace MessageBroker.Messages.State
{
    public struct ChangeExpEvent
    {
        public int Amount;
    }
    
    public struct ExpChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
}