using Core;
using Game.UI;
using Models.Player;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Album
{
    /// <summary>
    /// Страница работы над альбомом
    /// </summary>
    public class AlbumWorkingPage : Page
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
        [SerializeField] private AlbumResultPage albumResult;

        private AlbumInfo _album;
        
        /// <summary>
        /// Запускает создание нового альбома
        /// </summary>
        public void CreateAlbum(AlbumInfo album)
        {
            _album = album;
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
        /// Генерирует очки работы над альбомом
        /// </summary>
        private void GenerateWorkPoints()
        {
            var bitWorkPoints = CreateBitPoints(PlayerManager.Data);
            var textWorkPoints = CreateTextPoints(PlayerManager.Data);
            
            _album.BitPoints += bitWorkPoints;
            _album.TextPoints += textWorkPoints;
        }

        /// <summary>
        /// Создает очки работы по биту 
        /// </summary>
        private int CreateBitPoints(PlayerData data)
        {
            var playersBitPoints = Random.Range(1, data.Stats.Bitmaking + 1);
            playerBitWorkPoints.Show(playersBitPoints);

            var bitmakerPoints = 0;
            if (bitmaker.activeSelf)
            {
                bitmakerPoints = Random.Range(1, data.Team.BitMaker.Skill + 1);
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
            bitPoints.text = _album.BitPoints.ToString();
            textPoints.text = _album.TextPoints.ToString();
        }
        
        /// <summary>
        /// Завершает работу над треком
        /// </summary>
        private void FinishTrack()
        { 
            albumResult.Show(_album);
            Close();
        }
        
        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            header.text = $"Работа над альбомом \"{_album.Name}\"";
            bitmaker.SetActive(!PlayerManager.Data.Team.BitMaker.IsEmpty);
            textwritter.SetActive(!PlayerManager.Data.Team.TextWriter.IsEmpty);
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
            _album = null;
        }

        #endregion
    }
}