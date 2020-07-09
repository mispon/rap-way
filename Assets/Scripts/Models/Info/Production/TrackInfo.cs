using Enums;

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
    }
}