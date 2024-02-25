using System;
using Enums;

namespace Game.Player.Team.Desc
{
    /// <summary>
    /// Команда персонажа
    /// Не включаем сторонних чуваков, которые помогают при создании клипа
    /// </summary>
    [Serializable]
    public class Team
    {
        public Teammate BitMaker;
        public Teammate TextWriter;
        public Teammate Manager;
        public Teammate PrMan;

        public Teammate[] TeammatesArray => new[] {BitMaker, TextWriter, Manager, PrMan};

        public static Team New => new()
        {
            BitMaker = Teammate.New(TeammateType.BitMaker),
            TextWriter = Teammate.New(TeammateType.TextWriter),
            Manager = Teammate.New(TeammateType.Manager),
            PrMan = Teammate.New(TeammateType.PrMan)
        };
    }
}