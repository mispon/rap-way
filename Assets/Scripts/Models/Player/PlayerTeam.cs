using System;

namespace Models.Player
{
    /// <summary>
    /// Команда персонажа
    /// Не включаем сторонних чуваков, которые помогают при создании клипа
    /// </summary>
    [Serializable]
    public class PlayerTeam
    {
        public Teammate BitMaker;
        public Teammate TextWriter;
        public Teammate Producer;
        public Teammate SMM;

        public static PlayerTeam New => new PlayerTeam
        {
            BitMaker = Teammate.New,
            TextWriter = Teammate.New,
            Producer = Teammate.New,
            SMM = Teammate.New
        };
    }
}