using System;
using Enums;
using Models.Game;
using UnityEngine;

namespace Models.Player
{
    /// <summary>
    /// Тиммейт
    /// </summary>
    [Serializable]
    public class Teammate
    {
        public static Teammate New(TeammateType type) => new Teammate {Type = type};
        
        /// <summary>
        /// Тип тиммейта
        /// </summary>
        public TeammateType Type;

        /// <summary>
        /// Навык
        /// </summary>
        public ExpValue Skill;
        
        /// <summary>
        /// Флаг оплаты услуг тиммейта
        /// </summary>
        public bool HasPayment;

        /// <summary>
        /// Длительность восстановления тиммейта
        /// </summary>
        public int Cooldown;

        /// <summary>
        /// Тиммейт еще не открыт
        /// </summary>
        public bool IsEmpty => Skill.Value == 0;
    }
    
    [Serializable]
    public struct TeammateInfo
    {
        public TeammateType Type;
        public Sprite Avatar;
        public int FansToUnlock;
        public int[] Salary;
    }
}