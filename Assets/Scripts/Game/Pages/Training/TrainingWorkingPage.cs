using System;
using Core;
using Game.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Pages.Training
{
    /// <summary>
    /// Страница процесса тренировки
    /// </summary>
    public class TrainingWorkingPage : Page
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private WorkPoints playerWorkPoints;
        
        [Header("Страница результата")]
        [SerializeField] private TrainingResultPage trainingResult;

        private int _duration;
        private Func<string> _onFinish;
        
        /// <summary>
        /// Запускает процесс тренировки
        /// </summary>
        /// <param name="duration">Длительность тренировки</param>
        /// <param name="onFinish">Коллбэк успешного завершения</param>
        public void StartTrainig(int duration, Func<string> onFinish)
        {
            _duration = duration;
            _onFinish = onFinish;
            Open();
        }

        /// <summary>
        /// Обработчик истечения игрового дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;
            
            playerWorkPoints.Show(Random.Range(1, 4));
        }

        /// <summary>
        /// Обработчик завершения тренировки
        /// </summary>
        private void FinishTraining()
        {
            string message = _onFinish.Invoke();
            trainingResult.Show(message);
            Close();
        }

        #region PAGE CALLBACKS

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();
            
            progressBar.Init(_duration);
            progressBar.onFinish += FinishTraining;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();

            progressBar.onFinish -= FinishTraining;
        }

        #endregion
    }
}