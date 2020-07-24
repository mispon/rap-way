using System;

namespace Models.Game
{
    [Serializable]
    public struct ExpValue
    {
        /// <summary>
        /// Текущий уровень
        /// </summary>
        public int Value;

        /// <summary>
        /// Текущее значение опыта
        /// </summary>
        public int Exp;
    }
}