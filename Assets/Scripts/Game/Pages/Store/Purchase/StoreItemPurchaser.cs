using Data;
using MessageBroker.Messages.Goods;

namespace Game.Pages.Store.Purchase
{
    public static class StoreItemPurchaser
    {
        public static bool IsDonateCoins(GoodInfo item)
        {
            return item switch
            {
                DonateCoins => true,
                _ => false
            };
        }
        
        public static bool IsDonateItem(GoodInfo item)
        {
            return item switch
            {   
                DonateSwagGood => true,
                DonateEquipGood => true,
                _ => false
            };
        }

        public static AddNewGoodEvent CreateNewGoodEvent(GoodInfo item)
        {
            var goodEvent = new AddNewGoodEvent
            {
                Type = item.Type,
                Level = item.Level,
            };
            
            switch (item)
            {
                case SwagGood sg:
                    goodEvent.Hype = sg.Hype;
                    break;
                case DonateSwagGood dsg:
                    goodEvent.Hype = dsg.Hype;
                    break;
                case EquipGood eg:
                    goodEvent.QualityImpact = eg.QualityImpact;
                    break;
                case DonateEquipGood deg:
                    goodEvent.QualityImpact = deg.QualityImpact;
                    break;
            }

            return goodEvent;
        }
    }
}