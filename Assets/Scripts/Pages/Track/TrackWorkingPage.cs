using Core;
using Models.Production;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Pages.Track
{
    /// <summary>
    /// Страница работы над треком
    /// </summary>
    public class TrackWorkingPage : Page
    {
        [Header("Настройки")]
        [SerializeField] private int duration;

        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;
        
        [Header("Команда игрока")]
        [SerializeField] private GameObject bitmaker;
        [SerializeField] private GameObject textwritter;

        [Header("Страница результата")]
        [SerializeField] private TrackResultPage trackResult;

        private TrackInfo _track;
        private int _daysLeft;
    
        /// <summary>
        /// Запускает создание нового трека
        /// </summary>
        public void CreateTrack(TrackInfo track)
        {
            _track = track;
            _daysLeft = duration;
            Open();
        }
        
        /// <summary>
        /// Обработчик истечения игрового дня
        /// </summary>
        private void OnDayLeft()
        {
            if (_daysLeft == 0)
                return;
            
            _daysLeft--;

            progressBar.AddProgress(1);
            GenerateWorkPoints();
            DisplayWorkPoints();
        }
    
        /// <summary>
        /// Генерирует очки работы над треком
        /// </summary>
        private void GenerateWorkPoints()
        {
            _track.BitPoints += Random.Range(1, 5);
            _track.TextPoints += Random.Range(1, 5);
        }

        /// <summary>
        /// Отображает очки работы над треком
        /// </summary>
        private void DisplayWorkPoints()
        {
            bitPoints.text = _track.BitPoints.ToString();
            textPoints.text = _track.TextPoints.ToString();
        }

        /// <summary>
        /// Завершает работу над треком
        /// </summary>
        private void FinishTrack()
        { 
            trackResult.Show(_track);
            Close();
        }

        #region PAGE CALLBACKS

        protected override void AfterPageOpen()
        {
            progressBar.Init(duration);
            progressBar.onFinish += FinishTrack;
            
            TimeManager.Instance.SetActionMode();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.ResetActionMode();
            
            _track = null;
            progressBar.ResetProgress();
            progressBar.onFinish -= FinishTrack;
        }

        #endregion

        #region TIME LISTENER

        private void OnEnable()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
        }
        
        private void OnDisable()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
        }

        #endregion
    }
}