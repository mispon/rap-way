using System;

namespace Models.Player.DynamicData
{
    /// <summary>
    /// Изменяемые данные персонажа
    /// </summary>
    [Serializable]
    public class PlayerDynamicData
    {
        public Money Money;
        public Fans Fans;
        public Hype Hype;
    }
}