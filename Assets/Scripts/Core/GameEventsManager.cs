using System;
using Core.Interfaces;
using Data;
using Enums;
using Game;
using Game.Pages.GameEvent;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Core
{
    /// <summary>
    /// Контроллер игровых событий
    /// </summary>
    public class GameEventsManager: Singleton<GameEventsManager>, IStarter
    {
        [Header("Настройки")] 
        [SerializeField, Tooltip("Шанс выпадания события"), Range(0.001f, 1f)] 
        private float chance;

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
            if (chance >= Random.Range(0f, 1f))
            {
                int fans = PlayerManager.Data.Fans;
                GameEventInfo eventInfo = data.GetRandomInfo(type, fans);
                if (eventInfo != null)
                {
                    eventMainPage.Show(eventInfo);
                }
            }

            onEventShownAction.Invoke();
        }
    }
}