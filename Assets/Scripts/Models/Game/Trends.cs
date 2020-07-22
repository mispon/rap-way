using System;
using Core;
using Enums;

namespace Models.Game
{
    [Serializable]
    public class Trends
    {
        public Themes Theme;
        public Styles Style;
        public DateTime NextTimeUpdate;

        public static Trends New => new Trends
        {
            Style = Styles.Common,
            Theme = Themes.Self,
            NextTimeUpdate = TrendsManager.GetNextTimeUpdate(DateTime.Now)
        };
    }
}