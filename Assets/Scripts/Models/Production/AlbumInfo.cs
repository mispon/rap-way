using System;
using Core.Localization;
using Extensions;

namespace Models.Production
{
    /// <summary>
    /// Информация о выпущуенном альбоме
    /// Полная копия TrackInfo
    /// </summary>
    [Serializable]
    public class AlbumInfo : TrackInfo
    {
        public int ConcertAmounts;

        public override string[] HistoryInfo => new[]
        {
            Name,
            LocalizationManager.Instance.Get(TrendInfo.Style.GetDescription()),
            LocalizationManager.Instance.Get(TrendInfo.Theme.GetDescription()),
            $"{Convert.ToInt32(Quality * 100)}%",
            ListenAmount.GetDisplay(),
            ChartPosition > 0 ? ChartPosition.ToString() : "—"
        };

        public override string GetLog() {
            return $"{Timestamp}: {LocalizationManager.Instance.Get("log_album")} {Name}";
        }
    }
}