using System;
using Core;
using Core.OrderedStarter;
using Enums;
using ScriptableObjects;
using UI.Windows.GameScreen.GameEvent;
using UnityEngine;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Game
{
    /// <summary>
    /// Контроллер игровых событий
    /// </summary>
    public class GameEventsManager: Singleton<GameEventsManager>, IStarter
    {
        [Header("Настройки")] 
        [SerializeField, Tooltip("Шанс выпадания события"), Range(0.001f, 1f)] 
        private float chance;
        [SerializeField, Tooltip("Min tracks count to unlock")] 
        private int minTracksCount = 10;

        [Header("Страница события")] 
        [SerializeField] private EventMainPage eventMainPage;
        
        [Header("Данные")] 
        [SerializeField] private GameEventsData data;

        public void OnStart()
        {
            data.Initialize();
        }
        
        /// <summary>
        /// Вызывает с вероятностью случаное событие определенного типа
        /// </summary>
        public void CallEvent(GameEventType type, Action onEventShownAction)
        {
            if (PlayerAPI.Data.History.TrackList.Count < minTracksCount) {
                return;
            }

            if (chance >= Random.Range(0f, 1f))
            {
                var eventInfo = data.GetRandomInfo(type, PlayerAPI.State.GetFans());
                if (eventInfo != null)
                    eventMainPage.Show(eventInfo);
            }

            onEventShownAction.Invoke();
        }
    }
}