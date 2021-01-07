using System;
using Data;
using Enums;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Тренировочная карточка тематики
    /// </summary>
    public class TrainingThemeCard : TrainingToneCard
    {
        private Themes value => ((ThemesInfo) _info).Type;

        /// <summary>
        /// Возвращает значение 
        /// </summary>
        protected override Enum GetValue()
        {
            return value;
        }

        /// <summary>
        /// Возвращает признак закрытости 
        /// </summary>
        protected override bool IsLocked()
        {
            return !PlayerManager.Data.Themes.Contains(value);
        }
    }
}