using System;
using System.Linq;
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
        
        [Header("Данные сравнения")]
        [SerializeField] private TrendsCompareData trendsCompareData;
       
        private Trends Trends => GameManager.Instance.GameStats.Trends;
        private DateTime Now => TimeManager.Instance.Now;

        private static BaseCompareInfo<Styles>[] StyleCompareInfos => Instance.trendsCompareData.StylesCompareInfos;
        private static BaseCompareInfo<Themes>[] ThemesCompareInfos => Instance.trendsCompareData.ThemesCompareInfos;
        
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
        
        /// <summary>
        /// Анализирует совпадения выбранных стилей и темы с трендовыми
        /// </summary>
        public static void Analyze(TrendInfo info)
        {
            var currentTrend = GameManager.Instance.GameStats.Trends;
            
            var styleEquality = StyleCompareInfos.AnalyzeEquality(currentTrend.Style, info.Style);
            var themeEquality = ThemesCompareInfos.AnalyzeEquality(currentTrend.Theme, info.Theme);

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
                return 0.5f;

            var equalInfos = array.Where(el => el.IsEqualTo(currentValue));
            var equalInfosCount = equalInfos.Count();

            if (equalInfosCount == 0 || !equalInfos.Any(el => el.IsEqualTo(selectedValue)))
                return 0;

            return 0.5f / (equalInfosCount + 1);
        }
    }
}