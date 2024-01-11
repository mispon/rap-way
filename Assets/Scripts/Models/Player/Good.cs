using System;
using Enums;

namespace Models.Player
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