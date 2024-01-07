namespace MessageBroker.Messages.State
{
    public struct SpendDonateRequest
    {
        public int Amount;
    }

    public struct SpendDonateResponse
    {
        public bool OK;
    }

    public struct AddDonateEvent
    {
        public int Amount;
    }
    
    public struct DonateChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
}