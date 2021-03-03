using Data;
using Localization;
using Utils.Extensions;

namespace Models.Info.Production
{
    /// <summary>
    /// Информация о выпущенном треке
    /// </summary>
    [System.Serializable]
    public class TrackInfo: Production
    {
        public TrendInfo TrendInfo;
        public int TextPoints;
        public int BitPoints;
        public int ListenAmount;
        public int ChartPosition;
        public bool HasClip;
        public RapperInfo Feat;
        
        public override string[] HistoryInfo => new[]
        {
            Name,
            LocalizationManager.Instance.Get(TrendInfo.Style.GetDescription()),
            LocalizationManager.Instance.Get(TrendInfo.Theme.GetDescription()),
            ListenAmount.GetDisplay(),
            ChartPosition.ToString()
        };
        
        public override string GetLog() {
            return $"{Timestamp}: Завершил работу над треком {Name}";
        }
    }
}