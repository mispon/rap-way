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
        public StylesInfo[] StylesInfo;
        public ThemesInfo[] ThemesInfo;
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
        public Sprite Normal;
        public Sprite Locked;
    }

    [Serializable]
    public class TonesInfo
    {
        public int Price;
        public Sprite Normal;
        public Sprite Locked;
    }

    [Serializable]
    public class StylesInfo : TonesInfo
    {
        public Styles Type;
    }
    
    [Serializable]
    public class ThemesInfo : TonesInfo
    {
        public Themes Type;
    }
    
}