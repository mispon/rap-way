using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Game;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Контроль игрового времени
    /// </summary>
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    public class TimeManager : Singleton<TimeManager>
    {
        public DateTime Now { get; private set; }
        public string DisplayNow => Now.ToString("dd/MM/yyyy");

        [Header("Временные интервалы")]
        [SerializeField] private int actionInterval;
        [SerializeField] private int inactionInterval;

        public Action onDayLeft = () => {};
        public Action onWeekLeft = () => {};
        public Action onMonthLeft = () => {};

        private Coroutine _timer;
        private bool _hasAction;
        
        private WaitForSeconds _waitForSecondsActive;
        private WaitForSeconds _waitForSecondsInactive;

        private void Start()
        {
            _waitForSecondsActive = new WaitForSeconds(actionInterval);
            _waitForSecondsInactive = new WaitForSeconds(inactionInterval);
            
            Setup();
        }

        /// <summary>
        /// Инициализирует менеджер при старте игры 
        /// </summary>
        private void Setup()
        {
            Now = GameManager.Instance.GameStats.Now;
            _timer = StartCoroutine(TickCoroutine());
            
            EventManager.RaiseEvent(EventType.GameReady);
        }

        /// <summary>
        /// Устанавливает режим работы игрока
        /// </summary>
        public void SetActionMode()
        {
            _hasAction = true;
            RestartTimer();
        }

        /// <summary>
        /// Устанавливает режим бездействия игрока
        /// </summary>
        public void ResetActionMode()
        {
            _hasAction = false;
            RestartTimer();
        }
        
        /// <summary>
        /// Корутина игрового течения времени
        /// </summary>
        private IEnumerator TickCoroutine()
        {
            while (true)
            {
                yield return _hasAction ? _waitForSecondsActive : _waitForSecondsInactive;
                Tick();
            }
        }

        /// <summary>
        /// Обработчик завершения одного игрового дня 
        /// </summary>
        private void Tick()
        {
            Now = Now.AddDays(1);
            
            onDayLeft.Invoke();
            if (IsWeekLeft()) onWeekLeft.Invoke();
            if (IsMonthLeft()) onMonthLeft.Invoke();
        }

        private bool IsWeekLeft() => Now.DayOfWeek == DayOfWeek.Monday;
        private bool IsMonthLeft() => Now.Day == 1;

        /// <summary>
        /// Перезапускает корутину таймера
        /// </summary>
        private void RestartTimer()
        {
            StopCoroutine(_timer);
            _timer = StartCoroutine(TickCoroutine());
        }
        
        private void OnDisable()
        {
            StopCoroutine(_timer);
        }
    }
}