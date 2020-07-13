using System;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Тиммейт
    /// </summary>
    [Serializable]
    public class Teammate
    {
        public const int MAX_SKILL = 10;
        
        public static Teammate New(TeammateType type) => new Teammate {Type = type};
        
        /// <summary>
        /// Тип тиммейта
        /// </summary>
        public TeammateType Type;

        /// <summary>
        /// Флаг оплаты услуг тиммейта
        /// </summary>
        public bool HasPayment;
        
        /// <summary>
        /// Навык
        /// </summary>
        public int Skill;

        /// <summary>
        /// Тиммейт еще не открыт
        /// </summary>
        public bool IsEmpty => Skill == 0;
    }
    
    [Serializable]
    public struct TeammateInfo
    {
        public TeammateType Type;
        public int FansToUnlock;
        public int[] Salary;
    }
}