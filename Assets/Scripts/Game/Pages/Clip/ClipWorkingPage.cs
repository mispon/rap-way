using Core;
using Game.UI;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница работы над клипом
    /// </summary>
    public class ClipWorkingPage : Page
    {
        [Header("Настройки")]
        [SerializeField] private int duration;

        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text playerPoints;
        [SerializeField] private Text directorPoints;
        [SerializeField] private Text operatorPoints;

        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerWorkPoints;
        [SerializeField] private WorkPoints directorWorkPoints;
        [SerializeField] private WorkPoints operatorWorkPoints;

        [Header("Страница результата")]
        [SerializeField] private ClipResultPage clipResult;

        private ClipInfo _clip;
        
        /// <summary>
        /// Запускает создание нового клипа
        /// </summary>
        public void CreateClip(ClipInfo clip)
        {
            _clip = clip;
            header.text = $"Работа над клипом трека \"{PlayerManager.GetTrackName(_clip.TrackId)}\"";
            Open();
        }

        /// <summary>
        /// Обработчик истечения дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;
            
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Генерирует очки работы
        /// </summary>
        private void GenerateWorkPoints()
        {
            var playerPointsValue = Random.Range(1, PlayerManager.Data.Stats.Charisma);
            _clip.PlayerPoints += playerPointsValue;
            playerWorkPoints.Show(playerPointsValue);
            
            var directorPointsValue = Random.Range(1, _clip.DirectorSkill);
            _clip.DirectorPoints += directorPointsValue;
            directorWorkPoints.Show(directorPointsValue);
            
            var operatorPointsValue = Random.Range(1, _clip.OperatorSkill);
            _clip.OperatorPoints += operatorPointsValue;
            operatorWorkPoints.Show(operatorPointsValue);
        }

        /// <summary>
        /// Отображает количество сгенерированных очков работы
        /// </summary>
        private void DisplayWorkPoints()
        {
            playerPoints.text = _clip.PlayerPoints.ToString();
            directorPoints.text = _clip.DirectorPoints.ToString();
            operatorPoints.text = _clip.OperatorPoints.ToString();
        }

        /// <summary>
        /// Обработчик завершения клипа
        /// </summary>
        private void FinishClip()
        {
            clipResult.Show(_clip);
            Close();
        }
        
        #region PAGE CALLBACKS

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();
            
            progressBar.Init(duration);
            progressBar.onFinish += FinishClip;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();

            playerPoints.text = "0";
            directorPoints.text = "0";
            operatorPoints.text = "0";
            
            progressBar.onFinish -= FinishClip;
            _clip = null;
        }

        #endregion
    }
}