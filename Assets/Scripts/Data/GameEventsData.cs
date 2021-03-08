using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    /// <summary>
    /// Данные о событиях при создании Production
    /// </summary>
    [CreateAssetMenu(fileName = "GameEventsData", menuName = "Data/Game Events")]
    public class GameEventsData: ScriptableObject
    {
        private Dictionary<GameEventType, GameEventInfo[]> _gameEventInfosCollection;

        [SerializeField] private GameEventInfo[] trackEvents;
        [SerializeField] private GameEventInfo[] albumEvents;
        [SerializeField] private GameEventInfo[] clipEvents;
        [SerializeField] private GameEventInfo[] concertEvents;

        public void Initialize()
        {
            _gameEventInfosCollection = new Dictionary<GameEventType, GameEventInfo[]>
            {
                { GameEventType.Track, trackEvents },
                { GameEventType.Album, albumEvents },
                { GameEventType.Clip, clipEvents },
                { GameEventType.Concert, concertEvents }
            };
        }

        /// <summary>
        /// Возвращает случайное игровое событие
        /// Возвращает null, если не найдено событий этого типа
        /// </summary>
        public GameEventInfo GetRandomInfo(GameEventType type)
        {
            return _gameEventInfosCollection.TryGetValue(type, out var value) 
                ? value.GetRandom()
                : null;
        }
    }

    /// <summary>
    /// Данные конкретного события: выводимая пользователю информация и набор данных решений
    /// </summary>
    [Serializable]
    public class GameEventInfo
    {
        [Tooltip("Название события")]
        public string Name;
        
        [Tooltip("Описание события")]
        public string Description;
        
        [ArrayElementTitle("DecisionType")]
        [Tooltip("Набор данных, описывающих решение")]
        public GameEventDecision[] DecisionResults;
    }

    /// <summary>
    /// Данные решения: 
    /// </summary>
    [Serializable]
    public class GameEventDecision
    {
        [Tooltip("Тип решения")]
        public GameEventDecisionType DecisionType;

        [Tooltip("Описание события")]
        public string Description;
        
        [Tooltip("Величина изменения количества денег как % от текущего (от 1% до 15%)")]
        [Range(-0.15f, 0.15f)]
        public float MoneyChange;
        
        [Tooltip("Величина изменения количества фанов как % от текущего (от 1% до 10%)")]
        [Range(-0.1f, 0.1f)]
        public float FansChange;
        
        [Tooltip("Количество очков хайпа")]
        public int HypeChange;
        
        [Tooltip("Количество очков опыта")]
        public int ExpChange;
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Возвращает случайное событие из набора. Если набор пусто, то возвращает null
        /// </summary>
        public static GameEventInfo GetRandom(this GameEventInfo[] array)
        {
            return array.Length > 0 ? array[Random.Range(0, array.Length)] : null;
        }
    }
}