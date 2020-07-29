using System;
using Data;
using Enums;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Тренировочная карточка стилистики
    /// </summary>
    public class TrainingStyleCard : TrainingToneCard
    {
        private Styles value => ((StylesInfo) _info).Type;

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
            return !PlayerManager.Data.Styles.Contains(value);
        }
    }
}