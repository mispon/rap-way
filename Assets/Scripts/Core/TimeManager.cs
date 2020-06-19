using System;
using System.Timers;
using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Контроль игрового времени
    /// </summary>
    public class TimeManager : Singleton<TimeManager>
    {
        private DateTime _now;

        [Header("Временные интервалы")]
        [SerializeField] private int actionInterval;
        [SerializeField] private int inactionInterval;

        public DateTime Now => _now;

        public Action onDayLeft = () => {};
        public Action onWeekLeft = () => {};
        public Action onMonthLeft = () => {};

        private Timer _actionTimer;
        private Timer _inactionTimer;

        /// <summary>
        /// Инициализирует менеджер при старте игры 
        /// </summary>
        public void Setup(DateTime now)
        {
            _now = now;

            _actionTimer = new Timer(actionInterval * 1000) {AutoReset = true};
            _actionTimer.Elapsed += Tick;
            
            _inactionTimer = new Timer(inactionInterval * 1000) {AutoReset = true};
            _inactionTimer.Elapsed += Tick;
            
            SetActionMode();
        }

        /// <summary>
        /// Устанавливает режим работы игрока
        /// </summary>
        public void SetActionMode()
        {
            _inactionTimer.Stop();
            _actionTimer.Start();
        }

        /// <summary>
        /// Устанавливает режим бездействия игрока
        /// </summary>
        public void ResetActionMode()
        {
            _actionTimer.Stop();
            _inactionTimer.Start();
        }

        /// <summary>
        /// Обработчик завершения одного игрового дня 
        /// </summary>
        private void Tick(object sender, ElapsedEventArgs args)
        {
            _now = _now.AddDays(1);
            
            onDayLeft.Invoke();
            if (IsWeekLeft()) onWeekLeft.Invoke();
            if (IsMonthLeft()) onMonthLeft.Invoke();
        }

        private bool IsWeekLeft() => _now.DayOfWeek == DayOfWeek.Monday;
        private bool IsMonthLeft() => _now.Day == 1;

        private void OnDisable()
        {
            DisposeTimer(_actionTimer);
            DisposeTimer(_inactionTimer);
        }

        private static void DisposeTimer(Timer timer)
        {
            try
            {
                timer.Stop();
                timer.Dispose();    
            }catch{}
        }
    }
}