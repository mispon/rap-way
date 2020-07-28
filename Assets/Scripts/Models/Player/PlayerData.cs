using System;
using System.Collections.Generic;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Все данные игрока
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public const int MAX_SKILL = 10;
        
        public int Money;
        public int Fans;
        public int Hype;
        public int Exp;
        
        public PlayerInfo Info;
        public PlayerStats Stats;
        public PlayerHistory History;
        public PlayerTeam Team;
        public PlayerSocials Socials;
        
        public List<Good> Goods;
        public List<Achievement> Achievements;
        public List<Themes> Themes;
        public List<Styles> Styles;
        public List<Skills> Skills;
        
        public static PlayerData New => new PlayerData
        {
            Money =  500_000,
            Fans = 100_000,
            Exp = 500,
            
            Info = PlayerInfo.New,
            Stats = PlayerStats.New,
            History = PlayerHistory.New,
            Team = PlayerTeam.New,
            Socials = PlayerSocials.New,
            
            Goods = new List<Good>(),
            Achievements = new List<Achievement>(),
            Themes = new List<Themes> { Enums.Themes.Life },
            Styles = new List<Styles> { Enums.Styles.Underground },
            Skills = new List<Skills>()
        };
    }
}