using System;
using Models.Game;
using UnityEngine;

namespace Game.Labels.Desc
{
    [Serializable]
    public class LabelInfo
    {
        // Common
        public string Name;
        public string Desc;

        // Values
        public ExpValue Prestige;
        public ExpValue Production;
        public int      Score;

        // Activity cooldown (for AI)
        public int Cooldown;

        // Flags
        public bool IsCustom;
        // is custom player's label or not
        public bool IsPlayer;
        // is player's label frozen for non-payment or not
        public bool IsFrozen;

        // Logo
        [NonSerialized]
        public Sprite Logo;
        public string LogoName;
    }
}