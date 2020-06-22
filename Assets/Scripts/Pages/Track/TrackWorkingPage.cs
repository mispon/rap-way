using Core;
using Models.Production;
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

        [Header("Страница результата")]
        [SerializeField] private TrackResultPage trackResult;
        
        [Header("DEBUG")]
        [SerializeField] private Text daysLeft;

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
            _daysLeft--;
            
            // todo: generate work points
            // todo: update ui
            daysLeft.text = $"До завершения работы осталось {_daysLeft} дней";
            
            if (_daysLeft == 0)
                FinishTrack();
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
            TimeManager.Instance.SetActionMode();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.ResetActionMode();
            _track = null;
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