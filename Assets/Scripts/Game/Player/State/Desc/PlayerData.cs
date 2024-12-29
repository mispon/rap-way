using System;
using System.Collections.Generic;
using Enums;
using Game.Player.Achievements.Desc;
using Game.Player.Inventory.Desc;
using Game.Production.Desc;
using TrendsData = Models.Trends.Trends;

namespace Game.Player.State.Desc
{
    [Serializable]
    public class PlayerData
    {
        public const int MAX_SKILL = 10;

        public int    Money;
        public int    Donate;
        public int    Fans;
        public int    Hype;
        public int    Exp;
        public string Label;

        public PlayerInfo        Info;
        public PlayerStats       Stats;
        public ProductionHistory History;
        public Team.Desc.Team    Team;

        public List<InventoryItem> Inventory;
        public List<Achievement>   Achievements;
        public List<Themes>        Themes;
        public List<Styles>        Styles;
        public List<Skills>        Skills;
        public List<int>           Feats;
        public List<int>           Battles;

        public TrendsData LastKnownTrends;
        public bool       FinishPageShowed;

        public static PlayerData New => new()
        {
            Info    = PlayerInfo.New,
            Stats   = PlayerStats.New,
            History = ProductionHistory.New,
            Team    = Player.Team.Desc.Team.New,

            Inventory    = new List<InventoryItem>(),
            Achievements = new List<Achievement>(),
            Themes       = new List<Themes> {Enums.Themes.Life},
            Styles       = new List<Styles> {Enums.Styles.Underground},
            Skills       = new List<Skills>(),
            Feats        = new List<int>(),
            Battles      = new List<int>(),

            LastKnownTrends  = null,
            FinishPageShowed = false
        };
    }
}