using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Utils;
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

        /// <summary>
        /// Возвращает случайные данные решения по типу
        /// </summary>
        public GameEventDecision GetRandomDecision(GameEventDecisionType decisionType)
        {
            return DecisionResults.GetRandom(decisionType);
        }
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
        
        [Tooltip("Величина изменения количества денег как % от текущего (от 1% до 10%)")]
        [Range(-0.25f, 0.25f)]
        public float MoneyChange;
        
        [Tooltip("Величина изменения количества фанов как % от текущего (от 1% до 25%)")]
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
        public static GameEventInfo GetRandom(this GameEventInfo[] array) =>
            array.Length == 0 ? null : array[Random.Range(0, array.Length)];

        /// <summary>
        /// Возвращает случайное решение из типизированного набора решений, если таковые имеются.
        /// </summary>
        public static GameEventDecision GetRandom(this GameEventDecision[] gameEventDecisions, GameEventDecisionType decisionType)
        {
            var typedDecisions = gameEventDecisions.Where(el => el.DecisionType == decisionType).ToArray();
            if (typedDecisions.Length == 0)
                throw new RapWayException($"Нет ни одного решения типа \"{decisionType}\"");

            return typedDecisions[Random.Range(0, typedDecisions.Length)];
        }
    }
}