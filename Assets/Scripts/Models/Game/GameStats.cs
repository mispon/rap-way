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
        
        public static GameStats New => new GameStats
        {
            Now = DateTime.Now,
            Trands = new Trands {Style = Styles.Style0, Theme = Themes.Theme0}
        };
    }
}