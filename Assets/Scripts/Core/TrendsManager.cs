using System;
using Core.Interfaces;
using Data;
using Enums;
using Game;
using Game.Analyzers;
using Models.Game;
using Models.Info;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Класс управления трендами
    /// </summary>
    public class TrendsManager: Singleton<TrendsManager>, IStarter
    {
        private static readonly int STYLESCOUNT = Enum.GetValues(typeof(Styles)).Length;
        private static readonly int THEMESCOUNT = Enum.GetValues(typeof(Themes)).Length;
        
        private Trends Trends => GameManager.Instance.GameStats.Trends;
        private DateTime Now => TimeManager.Instance.Now;
        
        public void OnStart()
        {
            TimeManager.Instance.onWeekLeft += OnCheckTimeToChangeTrends;
        }

        /// <summary>
        /// Получение новой даты обновления трендов
        /// </summary>
        public static DateTime GetNextTimeUpdate(DateTime currentDate)
            => currentDate.AddMonths(UnityEngine.Random.Range(2, 4));

        /// <summary>
        /// Проверяем, наступило ли время для изменения Трендов
        /// </summary>
        private void OnCheckTimeToChangeTrends()
        {
            if (Now < Trends.NextTimeUpdate)
                return;
            
            ChangeTrends(Now);
        }

        /// <summary>
        /// Изменяем тренды
        /// </summary>
        private void ChangeTrends(DateTime now)
        {
            GameManager.Instance.GameStats.Trends = new Trends
            {
                Style = (Styles) UnityEngine.Random.Range(0, STYLESCOUNT),
                Theme = (Themes) UnityEngine.Random.Range(0, THEMESCOUNT),
                NextTimeUpdate = GetNextTimeUpdate(now)
            };
        }
    }
}