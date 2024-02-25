using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class SocialsSettings
    {
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 10;
        [Range(1, 30), Tooltip("Длительность отдыха PR-менеджера, дни")]
        public int Cooldown = 20;
        [Range(10, 100), Tooltip("Максимальное количество очков работы, шт")]
        public int WorkPointsMax = 70;
        [Range(0f, 1f), Tooltip("Сила влияния количества денег при пожертвовании")]
        public float CharitySizeImpact = 0.7f;
        [Range(0f, 1f), Tooltip("Доля активных фанатов от общего кол-ва, %")]
        public float ActiveFansGroup = 0.5f;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 100;
        [Range(1, 1000), Tooltip("Минимальная сумма баланса для возможности сделать пожертвование")]
        public int MinBalanceForCharity = 100;
    }
}