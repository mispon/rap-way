using System;
using Core;
using Core.Settings;
using Game.UI;
using UnityEngine;

namespace Game.Pages
{
    /// <summary>
    /// Базовая логика страниц работы
    /// </summary>
    public abstract class BaseWorkingPage : Page
    {
        [NonSerialized] protected GameSettings settings;
        [SerializeField] protected ProgressBar progressBar;

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
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();

            if (settings == null)
            {
                settings = GameManager.Instance.Settings;
            }

            progressBar.Init(GetDuration());
            progressBar.onFinish += FinishWork;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();
            
            progressBar.onFinish -= FinishWork;
        }
    }
}