using System;
using Localization;
using Utils.Extensions;

namespace Models.Info.Production
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
            return $"{Timestamp}: Завершил работу над альбомом {Name}";
        }
    }
}