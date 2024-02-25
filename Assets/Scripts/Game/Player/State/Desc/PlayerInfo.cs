using System;
using Enums;
using Extensions;

namespace Game.Player.State.Desc
{
    [Serializable]
    public class PlayerInfo
    {
        public string FirstName;
        public string LastName;
        public string NickName;
        public int Age;
        public Gender Gender;
        public string CreationDate;

        public static PlayerInfo New => new()
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            NickName = string.Empty,
            CreationDate = DateTime.Now.DateToString()
        };
    }
}