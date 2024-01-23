using System.ComponentModel;

namespace Enums
{
    public enum SocialType
    {
        [Description("social_trends")]
        Trends,
        [Description("social_twit")]
        Eagler,
        [Description("social_photo")]
        Ieyegram,
        [Description("social_story")]
        TackTack,
        [Description("social_translation")]
        Telescope,
        [Description("social_streamevent")]
        Switch,
        [Description("social_charity")]
        Charity
    }
}