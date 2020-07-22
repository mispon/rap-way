using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Стилистика трека / альбома
    /// </summary>
    public enum Styles
    {
        [Description("styles_common")]
        Common,
        [Description("styles_fast")]
        Fast,
        [Description("styles_slow")]
        Slow,
        [Description("styles_agressive")]
        Agressive,
        [Description("styles_lyric")]
        Lyric,
        [Description("styles_jerking")]
        Jerking,
        [Description("styles_reggae")]
        Reggae,
        [Description("styles_mumble")]
        Mumble
    }

    /// <summary>
    /// Тематика трека / альбома
    /// </summary>
    public enum Themes
    {
        [Description("theme_life")]
        Life,
        [Description("theme_money")]
        Money,
        [Description("theme_cars")]
        Cars,
        [Description("theme_girls")]
        Girls,
        [Description("theme_love")]
        Love,
        [Description("theme_success")]
        Success,
        [Description("theme_friends")]
        Friends,
        [Description("theme_haters")]
        Haters,
        [Description("theme_politic")]
        Politic,
        [Description("theme_self")]
        Self,
        [Description("theme_problems")]
        Problems,
        [Description("theme_motivation")]
        Motivation
    }
}