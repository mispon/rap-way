using System;

namespace Models.Player
{
    /// <summary>
    /// Команда персонажа
    /// Не включаем сторонних чуваков, которые помогают при создании [клипа/концерта/чего еще душе угодно]
    /// </summary>
    [Serializable]
    public class PlayerTeam//ToDo: Написать сериализатор для сохранения/загрузки
    {
        public Teammate BitMaker;
        public Teammate TextWriter;
        public Teammate Producer;
        public Teammate SMM;

        public static PlayerTeam Base => new PlayerTeam
        {
            BitMaker = Teammate.Base,
            TextWriter = Teammate.Base,
            Producer = Teammate.Base,
            SMM = Teammate.Base
        };
    }
}