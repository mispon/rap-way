using Data;
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
            TrendInfo.Style.GetDescription(),
            TrendInfo.Theme.GetDescription(),
            ListenAmount.GetDisplay(),
            ChartPosition.ToString()
        };
        
        public override string GetLog() {
            return $"{Timestamp}: Завершил работу над треком {Name}";
        }
    }
}