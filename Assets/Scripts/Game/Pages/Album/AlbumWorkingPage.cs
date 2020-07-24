using Models.Player;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Album
{
    /// <summary>
    /// Страница работы над альбомом
    /// </summary>
    public class AlbumWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
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
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _album = (AlbumInfo) args[0];
            Open();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            albumResult.Show(_album);
            Close();
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
            var playersBitPoints = Random.Range(1, data.Stats.Bitmaking.Value + 1);
            playerBitWorkPoints.Show(playersBitPoints);

            var bitmakerPoints = 0;
            if (bitmaker.activeSelf)
            {
                bitmakerPoints = Random.Range(1, data.Team.BitMaker.Skill.Value + 1);
                bitmakerWorkPoints.Show(bitmakerPoints);
            }

            return playersBitPoints + bitmakerPoints;
        }
        
        /// <summary>
        /// Создает очки работы по тексту 
        /// </summary>
        private int CreateTextPoints(PlayerData data)
        {
            var playersTextPoints = Random.Range(1, data.Stats.Vocobulary.Value + 1);
            playerTextWorkPoints.Show(playersTextPoints);

            var textwritterPoints = 0;
            if (textwritter.activeSelf)
            {
                textwritterPoints = Random.Range(1, data.Team.TextWriter.Skill.Value + 1);
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

        protected override void BeforePageOpen()
        {
            header.text = $"Работа над альбомом \"{_album.Name}\"";
            bitmaker.SetActive(!PlayerManager.Data.Team.BitMaker.IsEmpty);
            textwritter.SetActive(!PlayerManager.Data.Team.TextWriter.IsEmpty);
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            
            bitPoints.text = textPoints.text = "0";
            _album = null;
        }
    }
}