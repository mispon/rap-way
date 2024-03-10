using System;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Time;
using UI.Controls;
using UI.Controls.Progress;
using UniRx;
using UnityEngine;

namespace UI.Windows.GameScreen
{
    /// <summary>
    /// Базовая логика страниц работы
    /// </summary>
    public abstract class BaseWorkingPage : Page
    {
        [SerializeField] protected ProgressBar progressBar;

        private IDisposable _disposable;

        protected abstract void StartWork(object ctx);

        protected abstract void DoDayWork();

        protected abstract void FinishWork();

        protected abstract int GetDuration();

        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;

            DoDayWork();
        }

        protected void RefreshWorkAnims()
        {
            var anims = GetComponentsInChildren<ProductionAnim>();
            foreach (var anim in anims)
            {
                anim.Refresh();
            }
        }
        
        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance.Publish(new TimeActionModeMessage{ HasAction = true });
            
            _disposable = MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft());

            progressBar.Init(GetDuration());
            progressBar.onFinish += FinishWork;
            progressBar.Run();
        }

        protected override void BeforeHide()
        {
            MsgBroker.Instance.Publish(new TimeActionModeMessage{ HasAction = false });
            
            _disposable?.Dispose();
            
            progressBar.onFinish -= FinishWork;
        }
    }
}