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
        public List<RapperInfo> Rappers;
    }

    [Serializable]
    public class RapperInfo
    {
        public string Name;
        public int Id;
        public Sprite Avatar;
        public int Fans;
        public int Vocobulary;
        public int Bitmaking;
        public int Management;
        public bool IsCustom;
    }
}