using Enums;
using System;
using JetBrains.Annotations;

namespace Models.Player
{
    /// <summary>
    /// Контейнер дней до активции социального действия
    /// </summary>
    [Serializable]
    public class PlayerSocials
    {
        public SocialActivity TweetActivity;
        public SocialActivity PhotoActivity;
        public SocialActivity StoryActivity;
        public SocialActivity TranslationActivity;
        public SocialActivity StreamEventActivity;
        public SocialActivity CharityActivity;

        public SocialActivity[] AsArray => new[]
        {
            TweetActivity,
            PhotoActivity,
            StoryActivity,
            TranslationActivity,
            StreamEventActivity,
            CharityActivity
        };
        
        public static PlayerSocials New => new PlayerSocials
        {
            TweetActivity = new SocialActivity(),
            PhotoActivity = new SocialActivity(),
            StoryActivity = new SocialActivity(),
            TranslationActivity = new SocialActivity(),
            StreamEventActivity = new SocialActivity(),
            CharityActivity = new SocialActivity()
        };
    }
}

