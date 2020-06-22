using System;

namespace Models.Player.DynamicData
{
    /// <summary>
    /// Изменяемые данные персонажа
    /// </summary>
    [Serializable]
    public class PlayerDynamicData
    {
        public int Money;
        public int Fans;
        public int Hype;
        
        public static PlayerDynamicData New => new PlayerDynamicData();
    }
}