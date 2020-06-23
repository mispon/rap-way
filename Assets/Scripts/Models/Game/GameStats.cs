using System;
using Enums;

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
        public int Lang;
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trands = new Trands { Style = Styles.Common, Theme = Themes.Theme0 },
            Lang = 0
        };
    }
}