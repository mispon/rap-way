namespace MessageBroker.Messages.Player.State
{
    public struct SpendMoneyRequest
    {
        public int Amount;
    }

    public struct SpendMoneyResponse
    {
        public bool OK;
    }

    public struct ChangeMoneyMessage
    {
        public int Amount;
    }
    
    public struct MoneyChangedMessage
    {
        public int OldVal;
        public int NewVal;
    }
}