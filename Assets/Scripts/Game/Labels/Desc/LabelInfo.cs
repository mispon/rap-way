using System;
using Models.Game;
using UnityEngine;

namespace Game.Labels.Desc
{
    [Serializable]
    public class LabelInfo
    {
        public string   Name;
        public string   Desc;
        public ExpValue Prestige;
        public ExpValue Production;
        public int      Score;

        public bool IsCustom;

        // is custom player's label or not
        public bool IsPlayer;

        // is player's label frozen for non-payment or not
        public bool IsFrozen;

        [NonSerialized] public Sprite Logo;
    }
}