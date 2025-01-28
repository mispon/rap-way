using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Analytics;
using Core.Localization;
using Enums;
using Extensions;
using Game.Labels.Desc;
using Game.Rappers.Desc;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.Progress;
using UI.Controls.ScrollViewController;
using UI.Windows.GameScreen.Labels;
using UI.Windows.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Personal.LabelTab
{
    public class PlayersLabelSubTab : Tab
    {
        [SerializeField] private LabelTab     labelTab;
        [SerializeField] private AskingWindow askingWindow;
        [SerializeField] private Sprite       customLabelLogo;

        [Space]
        [SerializeField] private Image logo;

        [SerializeField] private Text   labelName;
        [SerializeField] private Text   exp;
        [SerializeField] private Text   income;
        [SerializeField] private Text   service;
        [SerializeField] private Button payServiceButton;

        [Space]
        [SerializeField] private int[] expToProductionLevelUp;

        [SerializeField] private Text        production;
        [SerializeField] private ProgressBar productionBar;
        [SerializeField] private Button      upProductionButton;

        [Space]
        [SerializeField] private int[] expToPrestigeLevelUp;

        [SerializeField] private PrestigeStars stars;
        [SerializeField] private ProgressBar   prestigeBar;
        [SerializeField] private Button        upPrestigeButton;

        [Space]
        [SerializeField] private ScrollViewController list;

        [SerializeField] private GameObject template;

        [Space]
        [SerializeField] private GameObject moneyReport;

        [SerializeField] private Text   moneyReportIncome;
        [SerializeField] private Text   moneyReportService;
        [SerializeField] private Text   moneyReportFrozenWarning;
        [SerializeField] private Button moneyReportOkButton;

        [Space]
        [SerializeField] private Button disbandButton;

        private readonly CompositeDisposable  _disposable = new();
        private readonly List<LabelMemberRow> _listItems  = new();

        private LabelInfo _label;
        private int       _income;
        private int       _cost;

        private const int minLabelCost = 250_000;
        private const int maxStatValue = 5;
        private const int expStep      = 100;

        private void Start()
        {
            payServiceButton.onClick.AddListener(PayService);
            upProductionButton.onClick.AddListener(UpProduction);
            upPrestigeButton.onClick.AddListener(UpPrestige);
            disbandButton.onClick.AddListener(DisbandLabel);
            moneyReportOkButton.onClick.AddListener(OnMoneyReportClose);
        }

        public void Show(LabelInfo label)
        {
            DisplayInfo(label);
            DisplayMembers(label);

            base.Open();

            HintsManager.Instance.ShowHint("tutorial_players_label", PersonalTabType.Label);
        }

        private void DisplayInfo(LabelInfo label)
        {
            _label = label;

            logo.sprite    = customLabelLogo;
            labelName.text = label.Name;

            production.text = label.Production.Value.ToString();
            productionBar.SetValue(label.Production.Exp, expToProductionLevelUp[label.Production.Value]);

            upProductionButton.gameObject.SetActive(label.Production.Value < maxStatValue);
            upProductionButton.interactable = PlayerAPI.Data.Exp >= expStep;

            var prestige = LabelsAPI.Instance.GetPrestige(label, expToPrestigeLevelUp);
            stars.Display(prestige);
            prestigeBar.SetValue(label.Prestige.Exp, expToPrestigeLevelUp[label.Prestige.Value]);

            upPrestigeButton.gameObject.SetActive(label.Prestige.Value < maxStatValue);
            upPrestigeButton.interactable = PlayerAPI.Data.Exp >= expStep;

            exp.text = PlayerAPI.Data.Exp.ToString();

            _income     = LabelsAPI.Instance.GetPlayersLabelIncome();
            income.text = _income.GetMoney();

            _cost        = GetServiceCost();
            service.text = _cost.GetMoney();

            payServiceButton.gameObject.SetActive(label.IsFrozen);
            payServiceButton.interactable = PlayerAPI.Data.Money >= _cost;
        }

        private void DisplayMembers(LabelInfo label)
        {
            var members = GetMembers(label.Name);

            for (var i = 0; i < members.Count; i++)
            {
                var info = members[i];

                var row = list.InstantiatedElement<LabelMemberRow>(template);
                row.Initialize(i + 1, info);

                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }

        /// <summary>
        ///     Returns all label members sorted desc by fans count
        /// </summary>
        private static List<RapperInfo> GetMembers(string labelName)
        {
            var members = RappersAPI.Instance
                .GetAll()
                .Where(e => e.Label == labelName)
                .ToList();

            members.Add(new RapperInfo
            {
                Name     = PlayerAPI.Data.Info.NickName,
                Fans     = PlayerAPI.Data.Fans,
                Label    = PlayerAPI.Data.Label,
                IsPlayer = true
            });

            return members.OrderByDescending(r => r.Fans).ToList();
        }

        private int GetServiceCost()
        {
            var prestige = LabelsAPI.Instance.GetPrestige(_label, expToPrestigeLevelUp);
            return Mathf.Max((int) (prestige * 1_000_000), minLabelCost);
        }

        private void PayService()
        {
            SoundManager.Instance.PlaySound(UIActionType.Pay);

            MsgBroker.Instance.Publish(new ChangeMoneyMessage {Amount = -_cost});
            _label.IsFrozen = false;

            DisplayInfo(_label);
        }

        private void UpProduction()
        {
            SoundManager.Instance.PlaySound(UIActionType.Train);

            var level   = _label.Production.Value;
            var expToUp = expToProductionLevelUp[level];

            var newExp = _label.Production.Exp + expStep;
            if (newExp >= expToUp)
            {
                SoundManager.Instance.PlaySound(UIActionType.LevelUp);
                newExp -= expToUp;
                level  += 1;
            }

            _label.Production.Value = level;
            _label.Production.Exp   = newExp;

            MsgBroker.Instance.Publish(new ChangeExpMessage {Amount = -expStep});
            DisplayInfo(_label);
        }

        private void UpPrestige()
        {
            SoundManager.Instance.PlaySound(UIActionType.Train);

            var level   = _label.Prestige.Value;
            var expToUp = expToPrestigeLevelUp[level];

            var newExp = _label.Prestige.Exp + expStep;
            if (newExp >= expToUp)
            {
                SoundManager.Instance.PlaySound(UIActionType.LevelUp);
                newExp -= expToUp;
                level  += 1;
            }

            _label.Prestige.Value = level;
            _label.Prestige.Exp   = newExp;

            MsgBroker.Instance.Publish(new ChangeExpMessage {Amount = -expStep});
            DisplayInfo(_label);
        }

        private void DisbandLabel()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.DisbandedOwnLabel);
            SoundManager.Instance.PlaySound(UIActionType.Click);

            askingWindow.Show(
                LocalizationManager.Instance.Get("disband_label_question").ToUpper(),
                () =>
                {
                    var members = GetMembers(_label.Name);
                    members.ForEach(e => e.Label = "");

                    PlayerAPI.Data.Label = "";
                    LabelsAPI.Instance.DisbandPlayersLabel();

                    labelTab.Reload();
                }
            );
        }

        public void ShowMoneyReport()
        {
            if (_label == null)
            {
                return;
            }
            
            var incomeValue = (_label.IsFrozen ? 0 : _income).GetMoney();
            moneyReportIncome.text = LocalizationManager.Instance
                .GetFormat("label_monthly_income", incomeValue)
                .ToUpper();

            var serviceValue = (_label.IsFrozen ? 0 : _cost).GetMoney();
            moneyReportService.text = LocalizationManager.Instance
                .GetFormat("label_monthly_service", serviceValue)
                .ToUpper();

            moneyReportFrozenWarning.gameObject.SetActive(_label.IsFrozen);
            moneyReport.SetActive(true);
        }

        private void OnMoneyReportClose()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            if (!_label.IsFrozen)
            {
                MsgBroker.Instance.Publish(new ChangeMoneyMessage {Amount = _income});
            }

            _label.IsFrozen = true;
            moneyReport.SetActive(false);

            DisplayInfo(_label);
        }

        public override void Close()
        {
            foreach (var row in _listItems)
            {
                Destroy(row.gameObject);
            }

            _listItems.Clear();
            _disposable.Clear();

            base.Close();
        }
    }
}