using System;
using Core;
using Core.OrderedStarter;
using Enums;
using ScriptableObjects;
using UnityEngine;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game
{
    /// <summary>
    ///     Контроллер игровых событий
    /// </summary>
    public class GameEventsManager : Singleton<GameEventsManager>, IStarter
    {
        [Header("Settings")]

        [Tooltip("Game event chance")]
        [SerializeField, Range(0.001f, 1f)]
        private float chance;

        [SerializeField]
        [Tooltip("Min tracks count to unlock")]
        private int minTracksCount = 10;

        [Header("Data")]
        [SerializeField] private GameEventsData data;

        public void OnStart()
        {
            data.Initialize();
        }

        public void CallEvent(GameEventType type, Action onEventShownAction)
        {
            if (PlayerAPI.Data.History.TrackList.Count >= minTracksCount)
            {
                if (chance >= Random.Range(0f, 1f))
                {
                    var eventInfo = data.GetRandomInfo(type, PlayerAPI.State.GetFans());
                    if (eventInfo != null)
                    {
                        MsgBroker.Instance.Publish(new WindowControlMessage
                        {
                            Type = WindowType.GameEvent,
                            Context = new Dictionary<string, object>
                            {
                                ["event_info"] = eventInfo,
                                ["close_callback"] = onEventShownAction,
                            }
                        });
                        return;
                    }
                }
            }

            onEventShownAction.Invoke();
        }
    }
}