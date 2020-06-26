using System;
using Enums;

namespace Models.Player
{
    /// <summary>
    /// Тиммейт
    /// Этим классом определяются как члены PlayerTeam, так и сторонние чуваки, которых мы нанимаем для создания [клипа/концерта/чего еще душе угодно]
    /// </summary>
    [Serializable]
    public class Teammate
    {
        public static Teammate New(Teammates type) => new Teammate {type = type};
        
        /// <summary>
        /// Тип тиммейта
        /// </summary>
        public Teammates type;

        /// <summary>
        /// Флаг оплаты услуг тиммейта.
        /// </summary>
        public bool hasPayment;
        
        /// <summary>
        /// Навык
        /// </summary>
        public int Skill;
        
        /// <summary>
        /// Тиммейт еще не открыт
        /// </summary>
        public bool IsEmpty => Skill == 0;
    }
    
    [System.Serializable]
    public struct TeammateInfo
    {
        public Teammates type;
        public int fansToUnlock;
        public int[] salary;
    }
}