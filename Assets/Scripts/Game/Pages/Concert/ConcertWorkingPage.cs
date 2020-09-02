using Core;
using Data;
using Enums;
using Models.Info.Production;
using Models.Player;
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
        [SerializeField] private Text managementPointsLabel;
        [SerializeField] private Text marketingPointsLabel;

        [Header("Команда игрока")] 
        [SerializeField] private WorkPoints playerManagementWorkPoints;
        [SerializeField] private WorkPoints playerPrWorkPoints;
        [SerializeField] private WorkPoints managerWorkPoints;
        [SerializeField] private WorkPoints prmanWorkPoints;
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Image prManAvatar;

        [Header("Данные")]
        [SerializeField] private ImagesBank imagesBank;        
        
        [Header("Страница результата")] 
        [SerializeField] private ConcertResultPage concertResult;

        private ConcertInfo _concert;
        private bool _hasManager;
        private bool _hasPrMan;

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
        /// Обработчик перехода к странице результата
        /// </summary>
        private void ShowResultPage()
        {
            concertResult.Show(_concert);
            Close();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            GameEventsManager.CallEvent(GameEventType.Concert, ShowResultPage);
        }

        /// <summary>
        /// Генерирует очки работы по организации концерта
        /// </summary>
        private void GenerateWorkPoints()
        {
            _concert.ManagementPoints += CreateManagementPoints(PlayerManager.Data);
            _concert.MarketingPoints += CreatePrPoints(PlayerManager.Data);
        }

        /// <summary>
        /// Создает очки работы менеджмента
        /// </summary>
        private int CreateManagementPoints(PlayerData data)
        {
            var playersManagementPoints = Random.Range(1, data.Stats.Management.Value + 1);
            playerManagementWorkPoints.Show(playersManagementPoints);

            var managerPoints = 0;
            if (_hasPrMan)
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

            var prManPoints = 0;
            if (_hasPrMan)
            {
                prManPoints = Random.Range(1, data.Team.PrMan.Skill.Value + 1);
                prmanWorkPoints.Show(prManPoints);
            }

            return playersMarketingPoints + prManPoints;
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
            _hasManager = TeamManager.IsAvailable(TeammateType.Manager);
            _hasPrMan = TeamManager.IsAvailable(TeammateType.PrMan);

            managerAvatar.sprite = _hasManager ? imagesBank.ProducerActive : imagesBank.ProducerInactive;
            prManAvatar.sprite = _hasPrMan ? imagesBank.PrManActive : imagesBank.PrManInactive;
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