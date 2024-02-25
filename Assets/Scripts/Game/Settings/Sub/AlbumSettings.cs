using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class AlbumSettings
    {
        [Range(5, 50), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 30;
        [Range(0.1f, 0.7f), Tooltip("Базовое качество трека, %")]
        public float BaseQuality = 0.15f;
        [Range(100, 400), Tooltip("Максимальное количество очков работы, шт")]
        public int WorkPointsMax = 400;
        [Range(0f, 1f), Tooltip("Вероятность хита, %")]
        public float HitChance = 0.15f;
        [Range(0f, 1f), Tooltip("Порог, выше которого альбом считается хитовым")]
        public float HitThreshold = 0.8f;
        [Range(0.001f, 0.1f), Tooltip("Цена одного прослушивания")]
        public float ListenCost = 0.017f;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 200;
        [Range(0.0f, 1.0f), Tooltip("Порог качества для попадания в чарты")]
        public float ChartsThreshold = 0.7f;
    }
}