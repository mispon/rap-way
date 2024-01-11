namespace MessageBroker.Messages.Production
{
    public struct ProductionRewardEvent
    {
        public int MoneyIncome;
        public int FansIncome;
        public int HypeIncome;
        public int Exp;
        public bool WithSocialCooldown;
    }

    public struct ConcertRewardEvent
    {
        public int MoneyIncome;
    }
}