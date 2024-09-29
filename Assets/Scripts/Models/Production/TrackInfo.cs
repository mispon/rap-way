using System;
using Core.Localization;
using Extensions;
using Game.Production;
using Game.Rappers.Desc;
using Models.Trends;

namespace Models.Production
{
    [Serializable]
    public class TrackInfo : ProductionBase
    {
        public TrendInfo TrendInfo;
        public int TextPoints;
        public int BitPoints;
        public int ListenAmount;
        public int ChartPosition;
        public bool HasClip;
        public int FeatId;

        public override string[] HistoryInfo => new[]
        {
            ProductionManager.GetTrackName(Id),
            LocalizationManager.Instance.Get(TrendInfo.Style.GetDescription()),
            LocalizationManager.Instance.Get(TrendInfo.Theme.GetDescription()),
            $"{Convert.ToInt32(Quality * 100)}%",
            ListenAmount.GetDisplay(),
            ChartPosition > 0 ? ChartPosition.ToString() : "—"
        };

        public override string GetLog()
        {
            return $"{Timestamp}: {LocalizationManager.Instance.Get("log_track")} {Name}";
        }
    }
}