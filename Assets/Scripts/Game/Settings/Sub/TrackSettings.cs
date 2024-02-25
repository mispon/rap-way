using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class TrackSettings
    {
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 15;
        [Range(5, 50), Tooltip("Длительность работы над фитом, дни")]
        public int FeatWorkDuration = 15;
        [Range(100, 300), Tooltip("Максимальное количество очков работы, шт")]
        public int WorkPointsMax = 230;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float HitChance = 0.1f;
        [Range(0f, 1f), Tooltip("Порог, выше которого трект считается хитом")]
        public float HitThreshold = 0.85f;
        [Range(0.001f, 0.05f), Tooltip("Цена одного прослушивания")]
        public float ListenCost = 0.01f;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 100;
        [Range(0.0f, 1.0f), Tooltip("Порог качества для попадания в чарты")]
        public float ChartsThreshold = 0.5f;
    }
}