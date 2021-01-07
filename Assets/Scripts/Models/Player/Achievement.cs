using System;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Достижения
    /// </summary>
    [Serializable]
    public class Achievement
    {
        /// <summary>
        /// Тип
        /// </summary>
        public AchievementsType Type;

        /// <summary>
        ///Необходимое значение для получение достижения
        /// </summary>
        public int CompareValue;

        /// <summary>
        /// Состояние: открыто/закрыто
        /// </summary>
        public bool Unlocked;
    }
}