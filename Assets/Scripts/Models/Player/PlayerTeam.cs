using System;
using Enums;

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

        public Teammate[] TeammatesArray => new[] {BitMaker, TextWriter, Producer, SMM};

        public static PlayerTeam New => new PlayerTeam
        {
            BitMaker = Teammate.New(TeammateType.BitMaker),
            TextWriter = Teammate.New(TeammateType.TextWriter),
            Producer = Teammate.New(TeammateType.Producer),
            SMM = Teammate.New(TeammateType.SMM)
        };
    }
}