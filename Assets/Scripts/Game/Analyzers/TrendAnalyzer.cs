using System.Linq;
using Data;
using Models.Info;
using UnityEngine;

namespace Game.Analyzers
{
    /// <summary>
    /// Анализатор совпадения выбранных стилей и темы с трендовыми
    /// </summary>
    public class TrendAnalyzer: Analyzer<TrendInfo>
    {
        [Header("Данные сравнения")]
        [SerializeField] private TrendsCompareData trendsCompareData;
        
        public override void Analyze(TrendInfo info)
        {
            var currentTrend = GameManager.Instance.GameStats.Trends;
            
            var styleEquality = trendsCompareData.StylesCompareInfos.AnalyzeEquality(currentTrend.Style, info.Style);
            var themeEquality = trendsCompareData.ThemesCompareInfos.AnalyzeEquality(currentTrend.Theme, info.Theme);

            info.EqualityValue = styleEquality + themeEquality;
        }
    }
    
    public static class Extension
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