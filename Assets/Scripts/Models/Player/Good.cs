using System;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Шмотка персонажа
    /// </summary>
    [Serializable]
    public class Good
    {
        /// <summary>
        /// Тип шмотки
        /// </summary>
        public GoodsType Type;
        
        /// <summary>
        /// Уровень прокачки шмотки
        /// </summary>
        public short Level;

        /// <summary>
        /// Бонус к хайпу
        /// </summary>
        public int Hype;
    }
}