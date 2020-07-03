using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Core.Interfaces;
using Game;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Контроль игрового времени
    /// </summary>
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    public class TimeManager : Singleton<TimeManager>, IStarter
    {
        public DateTime Now { get; private set; }
        public string DisplayNow => Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        [Header("Временные интервалы")]
        [SerializeField] private int actionInterval;
        [SerializeField] private int inactionInterval;

        public Action onDayLeft = () => {};
        public Action onWeekLeft = () => {};
        public Action onMonthLeft = () => {};

        private Coroutine _timer;
        private bool _hasAction;
        private bool _freezed;
        
        private WaitForSeconds _waitForSecondsActive;
        private WaitForSeconds _waitForSecondsInactive;

        public int SecondsPerTick => _hasAction ? actionInterval : inactionInterval;

        public void OnStart()
        {
            _waitForSecondsActive = new WaitForSeconds(actionInterval);
            _waitForSecondsInactive = new WaitForSeconds(inactionInterval);
            
            Now = GameManager.Instance.GameStats.Now;
            _timer = StartCoroutine(TickCoroutine());
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
        /// Устанавливает состояние заморозки времени 
        /// </summary>
        public void SetFreezed(bool state)
        {
            _freezed = state;
        }
        
        /// <summary>
        /// Корутина игрового течения времени
        /// </summary>
        private IEnumerator TickCoroutine()
        {
            while (true)
            {
                yield return new WaitWhile(() => _freezed);
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