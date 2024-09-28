using System;
using Game.Player.State.Desc;
using UnityEngine;

namespace Game.Rappers.Desc
{
    [Serializable]
    public class RapperInfo
    {
        public int Id;
        public string Name;
        public string Label;
        public int Fans;

        // Activity cooldown (for AI)
        public int Cooldown;

        // Skills 
        public int Vocobulary;
        public int Bitmaking;
        public int Management;

        // Flags
        public bool IsCustom;
        public bool IsPlayer;

        // Avatar
        public string AvatarName;
        [NonSerialized] public Sprite Avatar;

        // History
        public ProductionHistory History;
    }
}