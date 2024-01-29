using System;
using Enums;

namespace Models.Trends
{
    /// <summary>
    /// Информация о выбранных стиле и теме
    /// </summary>
    [Serializable]
    public class TrendInfo
    {
        public Styles Style;
        public Themes Theme;
        
        /// <summary>
        /// Величина совпадения стиля и темы с трендовыми
        /// </summary>
        public float EqualityValue;
    }
}