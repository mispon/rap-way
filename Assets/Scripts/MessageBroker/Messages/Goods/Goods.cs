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
    }
    
    public struct GoodExistsResponse
    {
        public bool Status;
    }
}