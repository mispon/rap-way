using Core;
using Enums;
using Firebase.Analytics;
using Game;
using Game.Labels;
using Game.Player;
using Models.Player;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.Pages.Concert
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
        [SerializeField] private WorkPoints labelManagementWorkPoints;
        [SerializeField] private WorkPoints labelPrWorkPoints;
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Image prManAvatar;
        [SerializeField] private Image labelAvatar;
        [SerializeField] private GameObject labelFrozen;
        
        [Header("Данные")]
        [SerializeField] private ImagesBank imagesBank;        
        
        [Header("Страница результата")] 
        [SerializeField] private ConcertResultPage concertResult;

        private ConcertInfo _concert;
        private bool _hasManager;
        private bool _hasPrMan;
        private LabelInfo _label;

        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.CreateConcertClick);
            
            _concert = (ConcertInfo) args[0];
            Open();
            RefreshWorkAnims();
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
            GameEventsManager.Instance.CallEvent(GameEventType.Concert, ShowResultPage);
        }

        /// <summary>
        /// Возвращает длительность действия
        /// </summary>
        protected override int GetDuration()
        {
            return settings.ConcertWorkDuration;
        }

        /// <summary>
        /// Генерирует очки работы по организации концерта
        /// </summary>
        private void GenerateWorkPoints()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);
            
            int managerPoints = CreateManagementPoints(PlayerManager.Data);
            int prPoints = CreatePrPoints(PlayerManager.Data);

            _concert.ManagementPoints += managerPoints;
            _concert.MarketingPoints += prPoints;
        }

        /// <summary>
        /// Создает очки работы менеджмента
        /// </summary>
        private int CreateManagementPoints(PlayerData data)
        {
            var playersManagementPoints = Random.Range(1, data.Stats.Management.Value + 2);
            playerManagementWorkPoints.Show(playersManagementPoints);

            var managerPoints = 0;
            if (_hasPrMan)
            {
                managerPoints = Random.Range(1, data.Team.Manager.Skill.Value + 2);
                managerWorkPoints.Show(managerPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelManagementWorkPoints.Show(labelPoints);
            }

            return playersManagementPoints + managerPoints + labelPoints;
        }

        /// <summary>
        /// Создает очки работы маркетинга
        /// </summary>
        private int CreatePrPoints(PlayerData data)
        {
            var playersMarketingPoints = Random.Range(1, data.Stats.Marketing.Value + 2);
            playerPrWorkPoints.Show(playersMarketingPoints);

            var prManPoints = 0;
            if (_hasPrMan)
            {
                prManPoints = Random.Range(1, data.Team.PrMan.Skill.Value + 2);
                prmanWorkPoints.Show(prManPoints);
            }
            
            var labelPoints = 0;
            if (_label is {IsFrozen: false})
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelPrWorkPoints.Show(labelPoints);
            }

            return playersMarketingPoints + prManPoints + labelPoints;
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
            base.BeforePageOpen();
            
            _hasManager = TeamManager.IsAvailable(TeammateType.Manager);
            _hasPrMan = TeamManager.IsAvailable(TeammateType.PrMan);

            managerAvatar.sprite = _hasManager ? imagesBank.ProducerActive : imagesBank.ProducerInactive;
            prManAvatar.sprite = _hasPrMan ? imagesBank.PrManActive : imagesBank.PrManInactive;
            
            if (!string.IsNullOrEmpty(PlayerManager.Data.Label))
            {
                _label = LabelsManager.Instance.GetLabel(PlayerManager.Data.Label);
            }

            if (_label != null)
            {
                labelAvatar.gameObject.SetActive(true);
                labelAvatar.sprite = _label.Logo;
                labelFrozen.SetActive(_label.IsFrozen);
            } else
            {
                labelAvatar.gameObject.SetActive(false);
            }
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