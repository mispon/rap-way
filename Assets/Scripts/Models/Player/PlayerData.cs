using System;
using Enums;
using Models.Player.DynamicData;

namespace Models.Player
{
    /// <summary>
    /// Все данные игрока
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public PlayerInfo Info;
        public PlayerStats Stats;
        public PlayerDynamicData Data;
        public PlayerHistory History;
        public PlayerTeam Team;
        
        public Good[] Goods;
        public Achievement[] Achievements;
        public Styles[] Styles;
        public Themes[] Themes;
        public Skills[] Skills;
    }
}