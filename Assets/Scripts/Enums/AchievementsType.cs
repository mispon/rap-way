using System.ComponentModel;

namespace Enums
{
    public enum AchievementsType
    {
        [Description("achievements_type_fans")]
        Fans,
        [Description("achievements_type_money")]
        Money,
        [Description("achievements_type_hype")]
        HypeBeast,

        [Description("achievements_type_trackchart")]
        TrackChartPosition,
        [Description("achievements_type_albumchart")]
        AlbumChartPosition,
        [Description("achievements_type_cliploser")]
        ClipLoser,
        [Description("achievements_type_concerplace")]
        ConcertPlace,

        [Description("achievements_type_feat")]
        Feat,
        [Description("achievements_type_battle")]
        Battle
    }
}