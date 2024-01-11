namespace MessageBroker.Messages.Donate
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
    
    public struct DonateAddedEvent {}
    
    public struct DonateChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
    
    public struct NoAdsPurchaseEvent {}
}