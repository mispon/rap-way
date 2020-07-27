using Enums;

namespace Models.Info
{
    /// <summary>
    /// Информация о выбранных стиле и теме
    /// </summary>
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