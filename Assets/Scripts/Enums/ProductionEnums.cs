using System.ComponentModel;

namespace Enums
{
    public enum Styles
    {
        [Description("styles_CloudRap")]
        CloudRap,
        [Description("styles_AfroBeat")]
        AfroBeat,
        [Description("styles_DownTempo")]
        DownTempo,
        [Description("styles_Dubstep")]
        Dubstep,
        [Description("styles_TripHop")]
        TripHop,
        [Description("styles_GlitchHop")]
        GlitchHop,
        [Description("styles_Underground")]
        Underground,
        [Description("styles_LoFi")]
        LoFi,
        [Description("styles_RapMetal")]
        RapMetal,
        [Description("styles_Rapcore")]
        Rapcore,
        [Description("styles_GettoFunk")]
        GettoFunk,
        [Description("styles_CountryRap")]
        CountryRap
    }
    
    public enum Themes
    {
        [Description("theme_life")]
        Life,
        [Description("theme_money")]
        Money,
        [Description("theme_girls")]
        Girls,
        [Description("theme_love")]
        Love,
        [Description("theme_friends")]
        Friends,
        [Description("theme_haters")]
        Haters,
        [Description("theme_politic")]
        Politic,
        [Description("theme_motivation")]
        Motivation,
        [Description("theme_fame")]
        Fame
    }
}