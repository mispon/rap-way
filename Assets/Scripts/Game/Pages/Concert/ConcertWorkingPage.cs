using Core;
using Game.UI;
using Models.Player;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница подготовки концерта
    /// </summary>
    public class ConcertWorkingPage : Page
    {
        [Header("Настройки")]
        [SerializeField] private int duration;

        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text managementPointsLabel;
        [SerializeField] private Text marketingPointsLabel;

        [Header("Команда игрока")]
        [SerializeField] private WorkPoints playerManagementWorkPoints;
        [SerializeField] private WorkPoints playerPrWorkPoints;
        [SerializeField] private WorkPoints managerWorkPoints;
        [SerializeField] private WorkPoints prmanWorkPoints;
        [SerializeField] private GameObject manager;
        [SerializeField] private GameObject prman;

        [Header("Страница результата")]
        [SerializeField] private ConcertResultPage concertResult;

        private ConcertInfo _concert;
        
        /// <summary>
        /// Запускает подготовку концерта
        /// </summary>
        public void CreateConcert(ConcertInfo concert)
        {
            _concert = concert;
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
        /// Генерирует очки работы по организации концерта
        /// </summary>
        private void GenerateWorkPoints()
        {
            var bitWorkPoints = CreateManagementPoints(PlayerManager.Data);
            var textWorkPoints = CreatePrPoints(PlayerManager.Data);
            
            _concert.ManagementPoints += bitWorkPoints;
            _concert.MarketingPoints += textWorkPoints;
        }

        /// <summary>
        /// Создает очки работы менеджмента
        /// </summary>
        private int CreateManagementPoints(PlayerData data)
        {
            var playersManagementPoints = Random.Range(1, data.Stats.Management);
            playerManagementWorkPoints.Show(playersManagementPoints);

            var managerPoints = 0;
            if (manager.activeSelf)
            {
                managerPoints = Random.Range(1, data.Team.Manager.Skill);
                managerWorkPoints.Show(managerPoints);
            }

            return playersManagementPoints + managerPoints;
        }
        
        /// <summary>
        /// Создает очки работы маркетинга
        /// </summary>
        private int CreatePrPoints(PlayerData data)
        {
            var playersMarketingPoints = Random.Range(1, data.Stats.Marketing);
            playerPrWorkPoints.Show(playersMarketingPoints);

            var prmanPoints = 0;
            if (manager.activeSelf)
            {
                prmanPoints = Random.Range(1, data.Team.PrMan.Skill);
                prmanWorkPoints.Show(prmanPoints);
            }

            return playersMarketingPoints + prmanPoints;
        }

        /// <summary>
        /// Отображает очки работы по организации концерта
        /// </summary>
        private void DisplayWorkPoints()
        {
            managementPointsLabel.text = _concert.ManagementPoints.ToString();
            marketingPointsLabel.text = _concert.MarketingPoints.ToString();
        }

        /// <summary>
        /// Завершает подготовку к концерту
        /// </summary>
        private void FinishConcert()
        {
            concertResult.Show(_concert);
            Close();
        }
        
        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            header.text = $"Организация концерта в \"{_concert.LocationName}\"";
            manager.SetActive(!PlayerManager.Data.Team.Manager.IsEmpty);
            prman.SetActive(!PlayerManager.Data.Team.PrMan.IsEmpty);
        }

        protected override void AfterPageOpen()
        {
            TimeManager.Instance.onDayLeft += OnDayLeft;
            TimeManager.Instance.SetActionMode();
            
            progressBar.Init(duration);
            progressBar.onFinish += FinishConcert;
            progressBar.Run();
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.onDayLeft -= OnDayLeft;
            TimeManager.Instance.ResetActionMode();

            managementPointsLabel.text = "0";
            marketingPointsLabel.text = "0";
            
            progressBar.onFinish -= FinishConcert;
            _concert = null;
        }

        #endregion
    }
}