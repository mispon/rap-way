using Enums;

namespace MessageBroker.Messages.Player
{
    public struct AddNewGoodMessage
    {
        public GoodsType Type;
        public short Level;
        public int Hype;
        public float QualityImpact;
    }

    public struct GoodExistsRequest
    {
        public GoodsType Type;
        public short Level;
        public bool IsNoAds;
    }
    
    public struct GoodExistsResponse
    {
        public bool Status;
    }
}