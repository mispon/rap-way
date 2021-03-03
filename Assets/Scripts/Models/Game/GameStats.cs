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
        // Глобальное состояние
        public DateTime Now;
        public Trends Trends;
        public int SocialsCooldown;
        
        // Настройки
        public GameLang Lang;
        public float SoundVolume;
        public float MusicVolume;
        public bool ShowAds;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trends = Trends.New,
            Lang = GameLang.RU,
            SoundVolume = 1f,
            MusicVolume = 1f,
            ShowAds = true
        };
    }
}