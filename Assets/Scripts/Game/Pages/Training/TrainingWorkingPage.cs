using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Pages.Training
{
    /// <summary>
    /// Страница процесса тренировки
    /// </summary>
    public class TrainingWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
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
        public override void StartWork(params object[] args)
        {
            _duration = (int) args[0];
            _onFinish = (Func<string>) args[1];
            Open();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            playerWorkPoints.Show(Random.Range(1, 11));
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            string message = _onFinish.Invoke();
            trainingResult.Show(message);
            Close();
        }

        protected override void AfterPageOpen()
        {
            duration = _duration;
            base.AfterPageOpen();
        }
    }
}