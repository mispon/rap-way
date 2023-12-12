using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Наборы данных о реальных исполнителях
    /// </summary>
    [CreateAssetMenu(fileName = "Rappers", menuName = "Data/Rappers")]
    public class RappersData : ScriptableObject
    {
        [ArrayElementTitle(new []{"Name"})]
        public List<RapperInfo> Rappers;
    }

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