using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Core;
using Core.OrderedStarter;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Time;
using UniRx;
using UnityEngine;

namespace Game.Time
{
    /// <summary>
    /// Контроль игрового времени
    /// </summary>
    [SuppressMessage("ReSharper", "IteratorNeverReturns")]
    public class TimeManager : Singleton<TimeManager>, IStarter
    {
        public DateTime Now { get; private set; }
        public string DisplayNow => Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        [Header("Временные интервалы, сек")]
        [SerializeField] private int actionInterval;
        [SerializeField] private int inactionInterval;
        
        private Coroutine _timer;
        private bool _hasAction;
        private bool _freezed;
        
        private WaitForSeconds _waitForSecondsActive;
        private WaitForSeconds _waitForSecondsInactive;

        private readonly CompositeDisposable _disposable;
        
        public void OnStart()
        {
            _waitForSecondsActive = new WaitForSeconds(actionInterval);
            _waitForSecondsInactive = new WaitForSeconds(inactionInterval);

            Now = GameManager.Instance.GameStats.Now.StringToDate();
            _timer = StartCoroutine(TickCoroutine());

            MsgBroker.Instance
                .Receive<TimeFreezeMessage>()
                .Subscribe(e => _freezed = e.IsFreezed)
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<TimeActionModeMessage>()
                .Subscribe(e =>
                {
                    _hasAction = e.HasAction;
                    RestartTimer();
                })
                .AddTo(_disposable);
        }

        private IEnumerator TickCoroutine()
        {
            while (true)
            {
                yield return new WaitWhile(() => _freezed);
                yield return _hasAction ? _waitForSecondsActive : _waitForSecondsInactive;
                Tick();
            }
        }
        
        private void Tick()
        {
            Now = Now.AddDays(1);
            
            MsgBroker.Instance.Publish(new DayLeftMessage {Day = Now.Day});
            
            if (IsWeekLeft())
            {
                int week = (Now.Day - 1) / 7 + 1;
                MsgBroker.Instance.Publish(new WeekLeftMessage {Week = week});
            }
            
            if (IsMonthLeft())
            {
                MsgBroker.Instance.Publish(new MonthLeftMessage {Month = Now.Month});   
            }
        }

        private bool IsWeekLeft() => Now.DayOfWeek == DayOfWeek.Monday;
        private bool IsMonthLeft() => Now.Day == 1;
        
        private void RestartTimer()
        {
            StopCoroutine(_timer);
            _timer = StartCoroutine(TickCoroutine());
        }
        
        private void OnDestroy()
        {
            if (_timer != null)
                StopCoroutine(_timer);   
            
            _disposable?.Clear();
        }
    }
}