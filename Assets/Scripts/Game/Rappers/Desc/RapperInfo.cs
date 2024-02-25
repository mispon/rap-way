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
        public Sprite Avatar;
        public int Fans;
        public int Vocobulary;
        public int Bitmaking;
        public int Management;
        public bool IsCustom;
        public bool IsPlayer;
    }
}