namespace MessageBroker.Messages.Production
{
    public struct ProductionRewardMessage
    {
        public int MoneyIncome;
        public int FansIncome;
        public int HypeIncome;
        public int Exp;
        public bool WithSocialCooldown;
    }

    public struct ConcertRewardMessage
    {
        public int MoneyIncome;
        public int Exp;
    }
}