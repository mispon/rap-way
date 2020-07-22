using Data;
using Enums;
using Utils.Extensions;

namespace Models.Info.Production
{
    /// <summary>
    /// Информация о выпущенном треке
    /// </summary>
    [System.Serializable]
    public class TrackInfo: Production
    {
        public Styles Style;
        public Themes Theme;

        public int TextPoints;
        public int BitPoints;

        public int ListenAmount;
        public int ChartPosition;

        public bool HasClip;

        public RapperInfo Feat;
        public override string[] HistoryInfo => new[]
        {
            Name,
            Style.GetDescription(),
            Theme.GetDescription(),
            ListenAmount.ToString(),
            ChartPosition.ToString()
        };
    }
}