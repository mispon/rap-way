using System;
using Enums;

using InputField = UnityEngine.UI.InputField;
using Dropdown = UnityEngine.UI.Dropdown;

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
        /// Дата рождения персонажа. Проще вычислять возрат от известной даты 
        /// </summary>
        public DateTime DateOfBirth;
        
        /// <summary>
        /// Дата создания персонажа, для презентации програсса игрока за определенный период
        /// </summary>
        public readonly DateTime CreationDate = DateTime.Now;
        
        /// <summary>
        /// Возраст персонажа
        /// ToDo: Когда будет создан TimeManager с полем Now (игровое время) необходимо будет заменить DateTime.Now на TimeManger.Now
        /// </summary>
        public int Age => (int) ((DateTime.Today - DateOfBirth).TotalDays / 365);

        /// <summary>
        /// Пол персонажа
        /// </summary>
        public Gender Gender;
        
        /// <summary>
        /// Раса персонажа
        /// </summary>
        public Race Race;
        
        /// <summary>
        /// Родина персонажа
        /// </summary>
        public string HomeLand;
        
        
        public override string ToString() => $"{FirstName} <{NickName}> {LastName}";
    }
}