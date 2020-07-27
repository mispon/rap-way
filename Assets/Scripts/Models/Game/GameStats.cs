using System;
using Enums;
using UnityEngine;

namespace Models.Game
{
    /// <summary>
    /// Данные игры
    /// </summary>
    [Serializable]
    public class GameStats
    {
        public DateTime Now;
        public Trends Trends;
        public SystemLanguage Lang;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trends = Trends.New,
            Lang = SystemLanguage.Russian
        };
    }
}