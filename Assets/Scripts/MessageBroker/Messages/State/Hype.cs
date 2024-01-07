namespace MessageBroker.Messages.State
{
    public struct ChangeHypeEvent
    {
        public int Amount;
    }
    
    public struct HypeChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
}