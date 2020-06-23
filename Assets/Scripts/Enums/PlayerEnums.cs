using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Пол
    /// </summary>
    public enum Gender { Male, Female }
    
    /// <summary>
    /// Раса
    /// </summary>
    public enum Race { Black, White, Yellow }
    
    /// <summary>
    /// Навыки
    /// </summary>
    public enum Skills { Skill0, Skill1 }

    /// <summary>
    /// Имущество
    /// </summary>
    public enum GoodsType { Micro }
    
    /// <summary>
    /// Достижения
    /// </summary>
    public enum AchievementsType { Ach0, Ach1 }

    /// <summary>
    /// Стилистика трека / альбома
    /// </summary>
    public enum Styles
    {
        [Description("styles_common")]
        Common,
        
        [Description("Агрессивный")]
        Agressive
    }

    /// <summary>
    /// Тематика трека / альбома
    /// </summary>
    public enum Themes
    {
        [Description("Theme0")]
        Theme0,
        
        [Description("Theme1")]
        Theme1
    }
}