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
        public Teammate Manager;
        public Teammate PrMan;

        public Teammate[] TeammatesArray => new[] {BitMaker, TextWriter, Manager, PrMan};

        public static PlayerTeam New => new()
        {
            BitMaker = Teammate.New(TeammateType.BitMaker),
            TextWriter = Teammate.New(TeammateType.TextWriter),
            Manager = Teammate.New(TeammateType.Manager),
            PrMan = Teammate.New(TeammateType.PrMan)
        };
    }
}