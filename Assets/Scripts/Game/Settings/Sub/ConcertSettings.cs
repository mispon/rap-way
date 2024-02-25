using System;
using UnityEngine;

namespace Game.Settings.Sub
{
    [Serializable]
    public class ConcertSettings
    {
        [Range(50, 500), Tooltip("Минимальное количество проданных билетов")]
        public int MinTicketsSold = 100;
        [Range(25, 75), Tooltip("Длительность работы, дни")]
        public int WorkDuration = 50;
        [Range(30, 90), Tooltip("Длительность отдыха после концерта, дни")]
        public int Cooldown = 60;
        [Range(200, 600), Tooltip("Максимальное количество очков работы, шт")]
        public int WorkPointsMax = 400;
        [Range(100, 1000), Tooltip("Количество очков опыта")]
        public int RewardExp = 400;
    }
}