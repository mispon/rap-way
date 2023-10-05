using System;
using Localization;
using Utils.Extensions;

namespace Models.Game
{
    /// <summary>
    /// Данные игры
    /// </summary>
    [Serializable]
    public class GameStats
    {
        /// <summary>
        /// Глобальное состояние
        /// </summary>
        public string Now;
        public Trends Trends;
        public int SocialsCooldown;
        public int ConcertCooldown;
        public bool AskedReview;
        
        /// <summary>
        /// Настройки игры
        /// </summary>
        public GameLang Lang;
        public float SoundVolume;
        public float MusicVolume;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now.DateToString(),
            Trends = Trends.New,
            Lang = GameLang.RU,
            SoundVolume = 1f,
            MusicVolume = 0.5f
        };
    }
}