using System;
using UnityEngine;
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

        /// <summary>
        /// Случайная генерация из предельных значений
        /// </summary>
        public PlayerStats(in DefaultPlayerStats defaultValues)
        {
            Vocobulary = Random.Range(1, defaultValues.maxVocobulary);
            Bitmaking = Random.Range(1, defaultValues.maxBitmaking);
            Flow = Random.Range(1, defaultValues.maxFlow);
            Charisma = Random.Range(1, defaultValues.maxCharisma);
        }
        
        public static PlayerStats New => new PlayerStats(new DefaultPlayerStats());
    }

    /// <summary>
    /// Можно настроить напрямую в инспекторе
    /// </summary>
    [Serializable]
    public struct DefaultPlayerStats
    {
        [Range(2, 10)] public int maxVocobulary;
        [Range(2, 10)] public int maxBitmaking;
        [Range(2, 10)] public int maxFlow;
        [Range(2, 10)] public int maxCharisma;
    }
}