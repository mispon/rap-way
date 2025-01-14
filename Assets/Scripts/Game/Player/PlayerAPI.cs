﻿using Enums;
using Game.Player.Inventory;
using Game.Player.State;
using Game.Player.State.Desc;
using Models.Trends;

namespace Game.Player
{
    public partial class PlayerPackage
    {
        public static PlayerData Data => GameManager.Instance.PlayerData;

        public static readonly StateAPI     State     = new();
        public static readonly InventoryAPI Inventory = new();

        public static void UpdateKnownTrends(Styles style, Themes theme)
        {
            Data.LastKnownTrends ??= Trends.New;

            Data.LastKnownTrends.Style = style;
            Data.LastKnownTrends.Theme = theme;
        }
    }
}