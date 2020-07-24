using System;
using Models.Game;

namespace Models.Player
{
    /// <summary>
    /// Статы персонажа
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        private const int MIN_VALUE = 1;
        
        /// <summary>
        /// Вокобуляр. Влияет на качество текстов для треков, альбомов и баттлов
        /// </summary>
        public ExpValue Vocobulary;
        
        /// <summary>
        /// В переводе не нуждается. Влияет на BitPoints в генераторе
        /// </summary>
        public ExpValue Bitmaking;
        
        /// <summary>
        /// Ораторское искусство, мастерство подачи текста
        /// Влияет на треки, альбомы, баттлы
        /// </summary>
        public ExpValue Flow;
        
        /// <summary>
        /// Харизма. Влияет на социальные действия, концерты и баттлы
        /// </summary>
        public ExpValue Charisma;

        /// <summary>
        /// Менеджмент. Влияет на организацию концертов
        /// </summary>
        public ExpValue Management;

        /// <summary>
        /// Маркетинг. Влияет на организацию концертов
        /// </summary>
        public ExpValue Marketing;

        public static PlayerStats New => new PlayerStats
        {
            Vocobulary = {Value = MIN_VALUE},
            Bitmaking = {Value = MIN_VALUE},
            Flow = {Value = MIN_VALUE},
            Charisma = {Value = MIN_VALUE},
            Management = {Value = MIN_VALUE},
            Marketing = {Value = MIN_VALUE}
        };

        /// <summary>
        /// Возвращает список актуальных значений навыков
        /// </summary>
        public ExpValue[] Values => new[]
        {
            Vocobulary,
            Bitmaking,
            Flow,
            Charisma,
            Management,
            Marketing
        };
    }
}