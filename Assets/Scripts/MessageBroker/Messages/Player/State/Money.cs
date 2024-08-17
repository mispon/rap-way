namespace MessageBroker.Messages.Player.State
{
    public struct SpendMoneyRequest
    {
        public string Id;
        public int Amount;
    }

    public struct SpendMoneyResponse
    {
        public string Id;
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