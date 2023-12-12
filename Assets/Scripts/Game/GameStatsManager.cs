using System;
using System.Linq;
using Core;
using Core.Interfaces;
using Data;
using Enums;
using Models.Game;
using Models.Info;
using UnityEngine;
using Utils;
// ReSharper disable CoVariantArrayConversion

namespace Game
{
    /// <summary>
    /// Класс управления трендами
    /// </summary>
    public class GameStatsManager: Singleton<GameStatsManager>, IStarter
    {
        private static readonly int STYLES_COUNT = Enum.GetValues(typeof(Styles)).Length;
        private static readonly int THEMES_COUNT = Enum.GetValues(typeof(Themes)).Length;
        
        [Header("Данные сравнения")]
        [SerializeField] private TrendsCompareData trendsCompareData;
        
        public void OnStart()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.onWeekLeft += OnWeekLeft;
        }
        
        /// <summary>
        /// Получение новой даты обновления трендов
        /// </summary>
        public static DateTime GetNextTimeUpdate(DateTime currentDate)
            => currentDate.AddMonths(UnityEngine.Random.Range(2, 4));

        /// <summary>
        /// Истечение игрового дня 
        /// </summary>
        private static void OnDayLeft()
        {
            var data = GameManager.Instance.GameStats;

            data.SocialsCooldown = Math.Max(0, data.SocialsCooldown - 1);
            data.ConcertCooldown = Math.Max(0, data.ConcertCooldown - 1);

            if (TimeManager.Instance.Now.Day % 3 == 0)
            {
                PlayerManager.Instance.AddHype(-1);
            }
        }

        /// <summary>
        /// Проверяем, наступило ли время для изменения Трендов
        /// </summary>
        private static void OnWeekLeft()
        {
            var now = TimeManager.Instance.Now;
            
            if (now < GameManager.Instance.GameStats.Trends.NextTimeUpdate)
                return;
            
            ChangeTrends(now);
        }

        /// <summary>
        /// Изменяем тренды
        /// </summary>
        private static void ChangeTrends(DateTime now)
        {
            GameManager.Instance.GameStats.Trends = new Trends
            {
                Style = (Styles) UnityEngine.Random.Range(0, STYLES_COUNT),
                Theme = (Themes) UnityEngine.Random.Range(0, THEMES_COUNT),
                NextTimeUpdate = GetNextTimeUpdate(now)
            };
        }

        /// <summary>
        /// Анализирует совпадения выбранных стилей и темы с трендовыми
        /// </summary>
        public static void Analyze(TrendInfo info)
        {
            var currentTrend = GameManager.Instance.GameStats.Trends;
            var compareData = Instance.trendsCompareData;
            
            var styleEquality = compareData.StylesCompareInfos.AnalyzeEquality(currentTrend.Style, info.Style);
            var themeEquality = compareData.ThemesCompareInfos.AnalyzeEquality(currentTrend.Theme, info.Theme);

            info.EqualityValue = styleEquality + themeEquality;
        }
    }
    
    public static partial class Extension
    {
        /// <summary>
        /// Получение оценки совпдаения выбранного и текущего значения
        /// </summary>
        public static float AnalyzeEquality<T>(this BaseCompareInfo<T>[] array, T currentValue, T selectedValue)
        {
            if (currentValue.Equals(selectedValue))
                return 0.1f;

            var equalInfos = array.Where(el => el.IsEqualTo(currentValue)).ToArray();
            var equalInfosCount = equalInfos.Length;

            if (equalInfosCount == 0 || !equalInfos.Any(el => el.IsEqualTo(selectedValue)))
                return 0;

            return 0.1f / (equalInfosCount + 1);
        }
    }
}