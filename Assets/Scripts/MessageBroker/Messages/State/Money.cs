namespace MessageBroker.Messages.State
{
    public struct SpendMoneyRequest
    {
        public int Amount;
    }

    public struct SpendMoneyResponse
    {
        public bool OK;
    }

    public struct AddMoneyEvent
    {
        public int Amount;
    }
    
    public struct MoneyChangedEvent
    {
        public int OldVal;
        public int NewVal;
    }
}