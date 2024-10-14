using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class RappersSettings
    {
        [Header("AI Base")]
        public int ConversationDuration = 30;
        public int MinFans    = 1;
        public int MaxFans    = 500_000_000;
        public int FeatChance = 10;

        [Header("AI Cooldowns")]
        public int TrackCooldown = 30;
        public int ClipCooldown    = 30;
        public int AlbumCooldown   = 50;
        public int ConcertCooldown = 100;
        public int EaglerCooldown  = 20;
        public int LabelCooldown   = 10;
        public int FeatCooldown    = 40;
        public int BattleCooldown  = 60;
    }
}