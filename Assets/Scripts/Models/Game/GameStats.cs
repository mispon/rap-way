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
        public Trands Trands;
        public SystemLanguage Lang;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trands = new Trands { Style = Styles.Common, Theme = Themes.Self },
            Lang = SystemLanguage.Russian
        };
    }
}