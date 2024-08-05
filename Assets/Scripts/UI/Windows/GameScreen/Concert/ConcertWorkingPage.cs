using Core;
using Core.Context;
using Enums;
using Firebase.Analytics;
using Game;
using Game.Labels.Desc;
using Game.Player.State.Desc;
using Game.Player.Team;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using UI.Enums;
using UI.Windows.Pages;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Concert
{
    public class ConcertWorkingPage : BaseWorkingPage
    {
        [Header("Work Points")]
        [SerializeField] private Text managementPointsLabel;
        [SerializeField] private Text marketingPointsLabel;

        [Header("Team")]
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

        [Header("Data"), SerializeField] private ImagesBank imagesBank;

        private ConcertInfo _concert;
        private bool _hasManager;
        private bool _hasPrMan;
        private LabelInfo _label;

        public override void Show(object ctx = null)
        {
            StartWork(ctx);
            base.Show(ctx);
        }

        protected override void StartWork(object ctx)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.CreateConcertClick);

            _concert = ctx.Value<ConcertInfo>();
            RefreshWorkAnims();
        }

        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        private void ShowResultPage()
        {
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionConcertResult,
                Context = _concert
            });
        }

        protected override void FinishWork()
        {
            GameEventsManager.Instance.CallEvent(GameEventType.Concert, ShowResultPage);
        }

        protected override int GetDuration()
        {
            return settings.Concert.WorkDuration;
        }

        private void GenerateWorkPoints()
        {
            SoundManager.Instance.PlaySound(UIActionType.WorkPoint);

            int managerPoints = CreateManagementPoints(PlayerAPI.Data);
            int prPoints = CreatePrPoints(PlayerAPI.Data);

            _concert.ManagementPoints += managerPoints;
            _concert.MarketingPoints += prPoints;
        }

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
            if (_label is { IsFrozen: false })
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelManagementWorkPoints.Show(labelPoints);
            }

            return playersManagementPoints + managerPoints + labelPoints;
        }

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
            if (_label is { IsFrozen: false })
            {
                labelPoints = Random.Range(1, _label.Production.Value + 1);
                labelPrWorkPoints.Show(labelPoints);
            }

            return playersMarketingPoints + prManPoints + labelPoints;
        }

        private void DisplayWorkPoints()
        {
            managementPointsLabel.text = _concert.ManagementPoints.ToString();
            marketingPointsLabel.text = _concert.MarketingPoints.ToString();
        }

        protected override void BeforeShow(object ctx = null)
        {
            base.BeforeShow(ctx);

            _hasManager = TeamManager.IsAvailable(TeammateType.Manager);
            _hasPrMan = TeamManager.IsAvailable(TeammateType.PrMan);

            managerAvatar.sprite = _hasManager ? imagesBank.ProducerActive : imagesBank.ProducerInactive;
            prManAvatar.sprite = _hasPrMan ? imagesBank.PrManActive : imagesBank.PrManInactive;

            if (!string.IsNullOrEmpty(PlayerAPI.Data.Label))
            {
                _label = LabelsAPI.Instance.Get(PlayerAPI.Data.Label);
            }

            if (_label != null)
            {
                labelAvatar.gameObject.SetActive(true);
                labelAvatar.sprite = _label.Logo;
                labelFrozen.SetActive(_label.IsFrozen);
            }
            else
            {
                labelAvatar.gameObject.SetActive(false);
            }
        }

        protected override void BeforeHide()
        {
            base.BeforeHide();

            managementPointsLabel.text = "0";
            marketingPointsLabel.text = "0";
            _concert = null;
        }
    }
}