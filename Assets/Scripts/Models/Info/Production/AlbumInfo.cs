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
        public override string[] HistoryInfo => new[]
        {
            Name,
            LocalizationManager.Instance.Get(TrendInfo.Style.GetDescription()),
            LocalizationManager.Instance.Get(TrendInfo.Theme.GetDescription()),
            ListenAmount.GetDisplay(),
            ChartPosition.ToString()
        };

        public override string GetLog() {
            return $"{Timestamp}: Завершил работу над альбомом {Name}";
        }
    }
}