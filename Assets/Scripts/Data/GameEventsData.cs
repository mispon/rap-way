using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    [CreateAssetMenu(fileName = "GameEventsData", menuName = "Data/Game Events")]
    public class GameEventsData: ScriptableObject
    {
        private readonly Dictionary<GameEventType, GameEventInfo[]> _gameEventInfosCollection = new Dictionary<GameEventType, GameEventInfo[]>();

        [SerializeField] private GameEventInfo[] trackGameEventInfos;
        [SerializeField] private GameEventInfo[] albumGameEventInfos;
        [SerializeField] private GameEventInfo[] clipGameEventInfos;
        [SerializeField] private GameEventInfo[] concertGameEventInfos;

        public void Initialize()
        {
            _gameEventInfosCollection.Add(GameEventType.Track, trackGameEventInfos);
            _gameEventInfosCollection.Add(GameEventType.Album, albumGameEventInfos);
            _gameEventInfosCollection.Add(GameEventType.Clip, clipGameEventInfos);
            _gameEventInfosCollection.Add(GameEventType.Concert, concertGameEventInfos);
        }

        /// <summary>
        /// Возвращает случайное игровое событие
        /// </summary>
        public GameEventInfo GetRandomInfo(GameEventType type)
        {
            if (!_gameEventInfosCollection.ContainsKey(type))
                return default;

            return _gameEventInfosCollection[type].GetRandom();
        }
    }

    [Serializable]
    public class GameEventInfo
    {
        [Tooltip("UI события")]
        public GameEventUi SituationUi;
        [ArrayElementTitle("DecisionType"), Tooltip("Набор данных, описывающих решение")]
        public GameEventDecision[] gameEventDecisions;

        /// <summary>
        /// Возвращает случайные данные решения по типу
        /// </summary>
        public GameEventDecision GetRandomDecision(GameEventDecisionType decisionType) 
            => gameEventDecisions.GetRandom(decisionType);
    }

    [Serializable]
    public class GameEventDecision
    {
        [Tooltip("Тип решения")]
        public GameEventDecisionType DecisionType;
        [Tooltip("Изменение метрик игрока в связи с выбором этого решения")]
        public MetricsIncome MetricsIncome;
        [Tooltip("UI решения")]
        public GameEventUi DecisionUi;
    }
    
    [Serializable]
    public struct GameEventUi
    {
        public string Description;
        public Sprite Background;
    }

    [Serializable]
    public struct MetricsIncome
    {
        public int Money;
        public int Fans;
        public int Hype;
        public int Experience;
    }

    public static partial class Extensions
    {
        public static GameEventInfo GetRandom(this GameEventInfo[] array)
        {
            if (array.Length == 0)
                return default;

            return array[Random.Range(0, array.Length)];
        }

        public static GameEventDecision GetRandom(this GameEventDecision[] gameEventDecisions, GameEventDecisionType decisionType)
        {
            var typedDecisions = gameEventDecisions.Where(el => el.DecisionType == decisionType).ToArray();
            if (typedDecisions.Length == 0)
                return default;

            return typedDecisions[Random.Range(0, typedDecisions.Length)];
        }
    }
}