using System;

namespace Models.Player
{
    /// <summary>
    /// Тиммейт
    /// Этим классом определяются как члены PlayerTeam, так и сторонние чуваки, которых мы нанимаем для создания [клипа/концерта/чего еще душе угодно]
    /// </summary>
    [Serializable]
    public class Teammate
    {
        public static Teammate Base => new Teammate();
        
        /// <summary>
        /// Навык
        /// </summary>
        public int Skill;
        
        /// <summary>
        /// Тиммейт еще не открыт
        /// </summary>
        public bool IsEmpty => Skill == 0;
    }
}