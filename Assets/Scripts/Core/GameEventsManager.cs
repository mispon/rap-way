using System;
using Core.Interfaces;
using Data;
using Enums;
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
        /// <summary>
        /// Функция обработки по завершении показа события
        /// </summary>
        public Action onEventShow = () => { }; 
        
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
        public static void CallEvent(GameEventType type, Action onEventShownAction)
        {
            if (Random.Range(0f, 1f) <= Instance.chance)
            {
                var eventInfo = Instance.data.GetRandomInfo(type);
                if (eventInfo != null)
                {
                    SetUpCallback(onEventShownAction);
                    Instance.eventMainPage.Show(eventInfo);
                    return;
                }
                
                Debug.LogAssertion($"Не добавлено ни одного игрового события типа \"{type}\"!");
            }
            onEventShownAction.Invoke();
        }

        /// <summary>
        /// Функция создания обработчика окончания показа игрвого события
        /// </summary>
        private static void SetUpCallback(Action onEventShownAction)
        {
            Instance.onEventShow = () =>
            {
                onEventShownAction?.Invoke();
                Instance.onEventShow = null;
            };
        }
    }
}