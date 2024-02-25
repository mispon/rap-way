using System;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Time;
using UI.Controls;
using UI.Controls.Progress;
using UniRx;
using UnityEngine;

namespace UI.Windows.Pages
{
    /// <summary>
    /// Базовая логика страниц работы
    /// </summary>
    public abstract class BaseWorkingPage : Page
    {
        [SerializeField] protected ProgressBar progressBar;

        private IDisposable _disposable;
        
        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public abstract void StartWork(params object[] args);

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected abstract void DoDayWork();

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected abstract void FinishWork();

        /// <summary>
        /// Возвращает длительность действия
        /// </summary>
        protected abstract int GetDuration();

        /// <summary>
        /// Вызывается по истечении игрового дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;

            DoDayWork();
        }

        /// <summary>
        /// Обновляет состояние аницаций работы
        /// </summary>
        protected void RefreshWorkAnims()
        {
            var anims = GetComponentsInChildren<ProductionAnim>();
            foreach (var anim in anims)
            {
                anim.Refresh();
            }
        }
        
        protected override void AfterPageOpen()
        {
            TimeManager.Instance.SetActionMode();
            _disposable = MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft());

            progressBar.Init(GetDuration());
            progressBar.onFinish += FinishWork;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.ResetActionMode();
            _disposable?.Dispose();
            
            progressBar.onFinish -= FinishWork;
        }
    }
}