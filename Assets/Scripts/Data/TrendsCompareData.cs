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
        [ArrayElementTitle(new [] { "value1", "value2" })] 
        public StylesCompareInfo[] StylesCompareInfos;
        
        [Header("Соотвествия схожих тем")]
        [ArrayElementTitle(new [] { "value1", "value2" })] 
        public ThemeCompareInfo[] ThemesCompareInfos;
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

        public bool IsEqualTo (T inputValue)
            => value1.Equals(inputValue) || value2.Equals(inputValue);
    }
}