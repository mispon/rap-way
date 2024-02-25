using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class BattleSettings
    {
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 20;
        [Range(1, 100), Tooltip("Количество очков хайпа за победу")]
        public int WinnerHype = 70;
        [Range(1, 100), Tooltip("Количество очков хайпа за проигрыш")]
        public int LoserHype = 25;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 500;
    }
}