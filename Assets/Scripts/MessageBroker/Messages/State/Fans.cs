namespace MessageBroker.Messages.State
{
    public struct ChangeFansEvent
    {
        public int Amount;
    }
    
    public struct FansChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
}