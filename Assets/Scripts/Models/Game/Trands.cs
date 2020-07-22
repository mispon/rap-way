using System;
using Core;
using Enums;

namespace Models.Game
{
    [Serializable]
    public class Trands
    {
        public Themes Theme;
        public Styles Style;
        public DateTime NextTimeUpdate;

        public static Trands New => new Trands
        {
            Style = Styles.Common,
            Theme = Themes.Self,
            NextTimeUpdate = TrandsManager.GetNextTimeUpdate(DateTime.Now)
        };
    }
}