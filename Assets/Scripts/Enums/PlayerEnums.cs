using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Пол персонажа
    /// </summary>
    public enum Gender
    {
        [Description("gender_male")]
        Male,
        [Description("gender_female")]
        Female
    }

    /// <summary>
    /// Раса персонажа
    /// </summary>
    public enum Race
    {
        [Description("race_white")]
        White,
        [Description("race_asian")]
        Asian,
        [Description("race_african")]
        African,
        [Description("race_latino")]
        Latino,
        [Description("race_afroamerican")]
        AfricanAmerican,
        [Description("race_indian")]
        Indian
    }

    /// <summary>
    /// Навыки персонажа
    /// </summary>
    public enum Skills
    {
        [Description("skill_autotune")]
        Autotune,
        [Description("skill_edlib")]
        Edlib,
        [Description("skill_doubletime")]
        DoubleTime,
        [Description("skill_reference")]
        Reference,
        [Description("skill_shoutout")]
        ShoutOut,
        [Description("skill_freestyle")]
        Freestyle,
        [Description("skill_punch")]
        Punch,
        [Description("skill_flip")]
        Flip,
        [Description("skill_dab")]
        Dab,
        [Description("skill_shootdance")]
        ShootDance
    }

    /// <summary>
    /// Имущество игрока
    /// </summary>
    public enum GoodsType
    {
        Micro, 
        AudioCard, 
        FxMixer, 
        Acustiq, 
        Car, 
        Appartaments, 
        Swatches, 
        Chain, 
        Grilz
    }
    
    /// <summary>
    /// Достижения игрока
    /// </summary>
    public enum AchievementsType { Ach0, Ach1 }

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

    /// <summary>
    /// Типы тиммейтов игрока
    /// </summary>
    public enum TeammateType
    {
        [Description("teammate_bitmaker")]
        BitMaker,
        [Description("teammate_textwritter")]
        TextWriter,
        [Description("teammate_manager")]
        Manager, 
        [Description("teammate_prman")]
        PrMan
    }

    /// <summary>
    /// Типы социальных действий
    /// </summary>
    public enum SocialType
    {
        [Description("social_twit")]
        Twit,
        [Description("social_photo")]
        Photo,
        [Description("social_story")]
        Story,
        [Description("social_translation")]
        Translation,
        [Description("social_streamevent")]
        StreamEvent,
        [Description("social_charity")]
        Charity
    }
}