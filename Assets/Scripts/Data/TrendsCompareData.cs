using System;
using System.Linq;
using Enums;
using Models.Game;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Данные для анализа схожести выбранных и текущих трендов
    /// </summary>
    [CreateAssetMenu(fileName = "TrendsCompare", menuName = "Data/Trends Compare")]
    public class TrendsCompareData: ScriptableObject
    {
        //TODO: Необходимо настроить классы совпадений!
        
        [Header("Соответсвия схожих стилей")]
        [SerializeField, ArrayElementTitle(new string[]{"value1", "value2"})] 
        private StylesCompareInfo[] stylesCompareInfos;
        
        [Header("Соотвествия схожих тем")]
        [SerializeField, ArrayElementTitle(new string[]{"value1", "value2"})] 
        private ThemeCompareInfo[] themesCompareInfos;

        /// <summary>
        /// Оценка совпадения текущего и выбранного трендов
        /// </summary>
        public float AnalyzeEquality(Trends currentTrends, Trends selectedTrends)
        {
            var styleEquality = stylesCompareInfos.AnalyzeEquality(currentTrends.Style, selectedTrends.Style);
            var themeEquality = themesCompareInfos.AnalyzeEquality(currentTrends.Theme, selectedTrends.Theme);

            return styleEquality + themeEquality;
        }
    }

    /// <summary>
    /// Информация схожих стилей
    /// </summary>
    [Serializable]
    public class StylesCompareInfo: BaseCompareInfo<Styles> {}
    /// <summary>
    /// Информация схожих тем
    /// </summary>
    [Serializable]
    public class ThemeCompareInfo: BaseCompareInfo<Themes> {}

    [Serializable]
    public class BaseCompareInfo<T>
    {
        [SerializeField] private T value1;
        [SerializeField] private T value2;

        public bool IsEqualTo<T>(T inputValue)
            => value1.Equals(inputValue) || value2.Equals(inputValue);
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