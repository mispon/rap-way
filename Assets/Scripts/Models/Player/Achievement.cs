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
        /// Хуй знает, как описать.
        /// Вместо того, чтобы создавать кучу перечислений типа Fans_1k -> Fans_100kk,
        /// Указываем в CompareValue 1000 -> 100000000, которое будет участвовать в функции проверки достижения
        /// </summary>
        public int CompareValue;

        /// <summary>
        /// Состояние: открыто/закрыто
        /// </summary>
        public bool Unlocked;
    }
}