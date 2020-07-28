using Core.Interfaces;
using Data;
using Enums;
using Game.Pages;
using Game.Pages.GameEvent;
using UnityEngine;
using Utils;

namespace Core
{
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
        public static void CallEvent(GameEventType type)
        {
            if (Random.Range(0f, 1f) > Instance.chance)
                return;

            Instance.eventMainPage.Show(Instance.data.GetRandomInfo(type));
        }
    }
}