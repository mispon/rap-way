using System.ComponentModel;

namespace Enums
{
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