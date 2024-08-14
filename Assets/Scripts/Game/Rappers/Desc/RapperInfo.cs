using System;
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
        public int Vocobulary;
        public int Bitmaking;
        public int Management;
        public bool IsCustom;
        public bool IsPlayer;
        public string AvatarName;

        [NonSerialized] public Sprite Avatar;
    }
}