using Enums;

namespace MessageBroker.Messages.Goods
{
    public struct AddNewGoodEvent
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

    public struct GoodsQualityImpactRequest {}

    public struct GoodsQualityImpactResponse
    {
        public float Value;
    }
}