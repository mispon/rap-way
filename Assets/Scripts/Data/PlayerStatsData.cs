using System;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Информация о навыках для отображения в интерфейсе
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Data/PlayerStats")]
    public class PlayerStatsData : ScriptableObject
    {
        public PlayerStatsInfo[] Items;
    }

    [Serializable]
    public class PlayerStatsInfo
    {
        public string NameKey;
        public string DescriptionKey;
    }
}