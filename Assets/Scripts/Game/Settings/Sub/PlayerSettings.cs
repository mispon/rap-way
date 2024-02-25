using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class PlayerSettings
    {
        [Range(100, 500), Tooltip("Базовое значение фанатов в анализаторе")]
        public int BaseFans = 300;
        [Tooltip("Максимальное кол-во фанатов, шт")]
        public int MaxFans = 500_000_000;
        [Tooltip("Минимальный прирост фанатов")]
        public int MinFansIncome = 10;
        [Tooltip("Максимальный прирост фанатов")]
        public int MaxFansIncome = 3_000_000;
        
        [Space]
        [Tooltip("Максимальное кол-во фанатов, шт")]
        public int MaxMoney = 2_000_000_000;
        [Tooltip("Минимальный прирост денег")]
        public int MinMoneyIncome = 10;
        [Tooltip("Максимальный прирост денег")]
        public int MaxMoneyIncome = 1_000_000;
        
        [Space]
        [Range(50_000, 300_000), Tooltip("Минимальное кол-во фанатов для участия в чартах")]
        public int MinFansForCharts = 150_000;
    }
}