using System;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Шмотка персонажа
    /// </summary>
    [Serializable]
    public struct Good
    {
        /// <summary>
        /// Тип шмотки
        /// </summary>
        public GoodsType Type;
        
        /// <summary>
        /// Уровень прокачки шмотки
        /// </summary>
        public short Level;
    }
}