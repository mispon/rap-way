using System;
using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Данные о навыках и умениях персонажа
    /// </summary>
    [CreateAssetMenu(fileName = "TrainingInfo", menuName = "Data/TrainingInfo")]
    public class TrainingInfoData : ScriptableObject
    {
        public PlayerStatsInfo[] StatsInfo;
        public PlayerSkillInfo[] SkillsInfo;
    }

    [Serializable]
    public class PlayerStatsInfo
    {
        public string NameKey;
        public string DescriptionKey;
    }
    
    [Serializable]
    public class PlayerSkillInfo
    {
        public string DescriptionKey;
        public Skills Type;
    }
}