using System;

namespace Models.Production
{
    [Serializable]
    public abstract class ProductionBase
    {
        public int    CreatorId;
        public int    Id;
        public string Name;
        public int    MoneyIncome;
        public int    FansIncome;
        public string Timestamp;
        public float  Quality;
        public bool   IsHit;

        public virtual string[] HistoryInfo => Array.Empty<string>();

        public abstract string GetLog();
    }
}