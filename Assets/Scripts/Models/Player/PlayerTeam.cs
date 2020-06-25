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

        public static PlayerTeam New => new PlayerTeam
        {
            BitMaker = Teammate.New(Teammates.BitMaker),
            TextWriter = Teammate.New(Teammates.TextWriter),
            Producer = Teammate.New(Teammates.Producer),
            SMM = Teammate.New(Teammates.SMM)
        }; 
        
        public Teammate[] TeammatesArray => new [] { BitMaker, TextWriter, Producer, SMM };
    }
}