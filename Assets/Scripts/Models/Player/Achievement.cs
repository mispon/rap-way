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
        /// Состояние: открыто/закрыто
        /// </summary>
        public bool Unlocked;
    }
}