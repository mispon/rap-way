using System;
using Enums;

namespace Game.Player.Goods.Desc
{
    [Serializable]
    public class Good
    {
        public GoodsType Type;
        public short Level;
        public int Hype;
        public float QualityImpact;
    }
}