namespace MessageBroker.Messages.Player
{
    public struct SpendDonateRequest
    {
        public int Amount;
    }

    public struct SpendDonateResponse
    {
        public bool OK;
    }

    public struct AddDonateMessage
    {
        public int Amount;
    }
    
    public struct DonateAddedMessage {}
    
    public struct DonateChangedMessage
    {
        public int OldVal;
        public int NewVal;
    }
    
    public struct NoAdsPurchaseMessage {}
}