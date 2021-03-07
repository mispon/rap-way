using System;
using Enums;
using Utils.Extensions;

namespace Models.Player
{
    /// <summary>
    /// Общая информация об персонаже
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        /// <summary>
        /// Имя персонажа
        /// </summary>
        public string FirstName;
        
        /// <summary>
        /// Фамилия персонажа
        /// </summary>
        public string LastName;
        
        /// <summary>
        /// Ник персонажа
        /// </summary>
        public string NickName;

        /// <summary>
        /// Возраст персонажа
        /// </summary>
        public int Age;

        /// <summary>
        /// Пол персонажа
        /// </summary>
        public Gender Gender;
        
        /// <summary>
        /// Дата создания персонажа, для презентации програсса игрока за определенный период
        /// </summary>
        public string CreationDate;

        public static PlayerInfo New => new PlayerInfo
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            NickName = string.Empty,
            CreationDate = DateTime.Now.DateToString()
        };
    }
}