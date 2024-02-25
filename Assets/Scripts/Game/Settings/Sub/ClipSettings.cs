using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class ClipSettings
    {
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 25;
        [Range(100, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int WorkPointsMax = 300;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float HitChance = 0.2f;
        [Range(0f, 1f), Tooltip("Порог, выше которого альбом считается хитовым")]
        public float HitThreshold = 0.7f;
        [Range(0f, 1f), Tooltip("Доля активных зрителей, %")]
        public float ActiveViewers = 0.3f;
        [Range(0.001f, 0.05f), Tooltip("Цена одного просмотра")]
        public float ViewCost = 0.01f;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 150;
    }
}