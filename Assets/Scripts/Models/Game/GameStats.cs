using System;
using Core.Localization;
using Extensions;
using UnityEngine;
using Application = UnityEngine.Device.Application;

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
        public Trends.Trends Trends;
        public int SocialsCooldown;
        public int ConcertCooldown;
        public bool AskedReview;
        public bool LangSelected;

        /// <summary>
        /// Настройки игры
        /// </summary>
        public GameLang Lang;

        public static GameStats New => new()
        {
            Now = DateTime.Now.DateToString(),
            Trends = Models.Trends.Trends.New,
            Lang = GetDeviceLang(),
        };

        private static GameLang GetDeviceLang()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Russian => GameLang.RU,
                SystemLanguage.Belarusian => GameLang.RU,
                SystemLanguage.Ukrainian => GameLang.RU,
                SystemLanguage.Portuguese => GameLang.PT,
                SystemLanguage.Spanish => GameLang.ES,
                SystemLanguage.Italian => GameLang.IT,
                SystemLanguage.French => GameLang.FR,
                SystemLanguage.German => GameLang.DE,
                _ => GameLang.EN
            };
        }
    }
}