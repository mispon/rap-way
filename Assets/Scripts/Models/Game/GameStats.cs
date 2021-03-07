using System;
using Localization;

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
        public DateTime Now;
        public Trends Trends;
        public int SocialsCooldown;
        
        /// <summary>
        /// Настройки игры
        /// </summary>
        public GameLang Lang;
        public float SoundVolume;
        public float MusicVolume;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trends = Trends.New,
            Lang = GameLang.RU,
            SoundVolume = 1f,
            MusicVolume = 1f
        };
    }
}