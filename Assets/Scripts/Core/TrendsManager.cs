using System;
using Core.Interfaces;
using Data;
using Enums;
using Game;
using Models.Game;
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

        [Header("Данные сравнения")]
        [SerializeField] private TrendsCompareData trendsCompareData;
        
        private Trends Trends => GameManager.Instance.GameStats.trends;
        private DateTime Now => TimeManager.Instance.Now;
        
        public void OnStart()
        {
            TimeManager.Instance.onWeekLeft += OnCheckTimeToChangeTrands;
        }

        /// <summary>
        /// Получение новой даты обновления трендов
        /// </summary>
        public static DateTime GetNextTimeUpdate(DateTime currentDate)
            => currentDate.AddMonths(UnityEngine.Random.Range(2, 4));

        /// <summary>
        /// Проверяем, наступило ли время для изменения Трендов
        /// </summary>
        private void OnCheckTimeToChangeTrands()
        {
            if (Now < Trends.NextTimeUpdate)
                return;
            
            ChangeTrands(Now);
        }

        /// <summary>
        /// Изменяем тренды
        /// </summary>
        private void ChangeTrands(DateTime now)
        {
            GameManager.Instance.GameStats.trends = new Trends
            {
                Style = (Styles) UnityEngine.Random.Range(0, STYLESCOUNT),
                Theme = (Themes) UnityEngine.Random.Range(0, THEMESCOUNT),
                NextTimeUpdate = GetNextTimeUpdate(now)
            };
        }

        /// <summary>
        /// Получение оценки, насколько точное совпадение по текущим трендам
        /// </summary>
        public static float AnalyzeEquality(Trends selectedTrend)
            => Instance.trendsCompareData.AnalyzeEquality(Instance.Trends, selectedTrend);
    }
}