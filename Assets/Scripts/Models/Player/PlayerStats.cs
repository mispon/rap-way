using System;
using Random = UnityEngine.Random;

namespace Models.Player
{
    /// <summary>
    /// Статы персонажа
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        /// <summary>
        /// Вокобуляр. Влияет на качество текстов для треков, альбомов и баттлов
        /// </summary>
        public int Vocobulary;
        
        /// <summary>
        /// В переводе не нуждается. Влияет на BitPoints в генераторе
        /// </summary>
        public int Bitmaking;
        
        /// <summary>
        /// Ораторское искусство, мастерство подачи текста
        /// Влияет на треки, альбомы, баттлы
        /// </summary>
        public int Flow;
        
        /// <summary>
        /// Харизма. Влияет на социальные действия, концерты и баттлы
        /// </summary>
        public int Charisma;

        /// <summary>
        /// Менеджмент. Влияет на организацию концертов
        /// </summary>
        public int Management;

        /// <summary>
        /// Маркетинг. Влияет на организацию концертов
        /// </summary>
        public int Marketing;

        public static PlayerStats New => new PlayerStats
        {
            Vocobulary = RandomValue,
            Bitmaking = RandomValue,
            Flow = RandomValue,
            Charisma = RandomValue,
            Management = RandomValue,
            Marketing = RandomValue
        };

        /// <summary>
        /// Возвращает список актуальных значений навыков
        /// </summary>
        public int[] Values => new[] { Vocobulary, Bitmaking, Flow, Charisma, Management, Marketing };

        /// <summary>
        /// Случайный разброс начальных характеристик от 1 до 2
        /// </summary>
        private static int RandomValue => Random.Range(1, 3);
    }
}