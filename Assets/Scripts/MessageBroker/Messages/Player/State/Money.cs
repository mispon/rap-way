namespace MessageBroker.Messages.Player.State
{
    public struct SpendMoneyRequest
    {
        public string Source;
        public int    Amount;
    }

    public struct SpendMoneyResponse
    {
        public string Source;
        public bool   OK;
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