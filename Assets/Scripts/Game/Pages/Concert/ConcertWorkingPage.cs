using Models.Player;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница подготовки концерта
    /// </summary>
    public class ConcertWorkingPage : BaseWorkingPage
    {
        [Header("Идентификаторы прогресса работы")]
        [SerializeField] private Text header;
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
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _concert = (ConcertInfo) args[0];
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
            concertResult.Show(_concert);
            Close();
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
            var playersManagementPoints = Random.Range(1, data.Stats.Management.Value + 1);
            playerManagementWorkPoints.Show(playersManagementPoints);

            var managerPoints = 0;
            if (manager.activeSelf)
            {
                managerPoints = Random.Range(1, data.Team.Manager.Skill.Value + 1);
                managerWorkPoints.Show(managerPoints);
            }

            return playersManagementPoints + managerPoints;
        }
        
        /// <summary>
        /// Создает очки работы маркетинга
        /// </summary>
        private int CreatePrPoints(PlayerData data)
        {
            var playersMarketingPoints = Random.Range(1, data.Stats.Marketing.Value + 1);
            playerPrWorkPoints.Show(playersMarketingPoints);

            var prmanPoints = 0;
            if (manager.activeSelf)
            {
                prmanPoints = Random.Range(1, data.Team.PrMan.Skill.Value + 1);
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

        protected override void BeforePageOpen()
        {
            header.text = $"Организация концерта в \"{_concert.LocationName}\"";
            manager.SetActive(!PlayerManager.Data.Team.Manager.IsEmpty);
            prman.SetActive(!PlayerManager.Data.Team.PrMan.IsEmpty);
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();

            managementPointsLabel.text = "0";
            marketingPointsLabel.text = "0";
            _concert = null;
        }
    }
}