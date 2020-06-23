using Core;
using Models.Player;
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
        [SerializeField] private Text header;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text bitPoints;
        [SerializeField] private Text textPoints;

        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerBitWorkPoints;
        [SerializeField] private WorkPoints playerTextWorkPoints;
        [SerializeField] private WorkPoints bitmakerWorkPoints;
        [SerializeField] private WorkPoints textwritterWorkPoints;
        [SerializeField] private GameObject bitmaker;
        [SerializeField] private GameObject textwritter;

        [Header("Страница результата")]
        [SerializeField] private TrackResultPage trackResult;

        private TrackInfo _track;
    
        /// <summary>
        /// Запускает создание нового трека
        /// </summary>
        public void CreateTrack(TrackInfo track)
        {
            _track = track;
            Open();
        }
        
        /// <summary>
        /// Обработчик истечения игрового дня
        /// </summary>
        private void OnDayLeft()
        {
            if (progressBar.IsFinish)
                return;
            
            GenerateWorkPoints();
            DisplayWorkPoints();
        }
    
        /// <summary>
        /// Генерирует очки работы над треком
        /// </summary>
        private void GenerateWorkPoints()
        {
            var bitWorkPoints = CreateBitPoints(PlayerManager.PlayerData);
            var textWorkPoints = CreateTextPoints(PlayerManager.PlayerData);
            
            _track.BitPoints += bitWorkPoints;
            _track.TextPoints += textWorkPoints;
        }

        /// <summary>
        /// Создает очки работы по биту 
        /// </summary>
        private int CreateBitPoints(PlayerData data)
        {
            var playersBitPoints = Random.Range(1, data.Stats.Bitmaking);
            playerBitWorkPoints.Show(playersBitPoints);

            var bitmakerPoints = 0;
            if (bitmaker.activeSelf)
            {
                bitmakerPoints = Random.Range(1, data.Team.BitMaker.Skill);
                bitmakerWorkPoints.Show(bitmakerPoints);
            }

            return playersBitPoints + bitmakerPoints;
        }
        
        /// <summary>
        /// Создает очки работы по тексту 
        /// </summary>
        private int CreateTextPoints(PlayerData data)
        {
            var playersTextPoints = Random.Range(1, data.Stats.Vocobulary);
            playerTextWorkPoints.Show(playersTextPoints);

            var textwritterPoints = 0;
            if (textwritter.activeSelf)
            {
                textwritterPoints = Random.Range(1, data.Team.TextWriter.Skill);
                textwritterWorkPoints.Show(textwritterPoints);
            }

            return playersTextPoints + textwritterPoints;
        }

        /// <summary>
        /// Обновляет значение рабочих очков в интерфейсе
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

        protected override void BeforePageOpen()
        {
            header.text = $"Работа над треком \"{_track.Name}\"";
            bitmaker.SetActive(!PlayerManager.PlayerData.Team.BitMaker.IsEmpty);
            textwritter.SetActive(!PlayerManager.PlayerData.Team.TextWriter.IsEmpty);
        }

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();
            
            progressBar.Init(duration);
            progressBar.onFinish += FinishTrack;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();

            bitPoints.text = textPoints.text = "0";
            
            progressBar.onFinish -= FinishTrack;
            _track = null;
        }

        #endregion
    }
}