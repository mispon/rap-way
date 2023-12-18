using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.Pages.Labels;
using Game.UI;
using Game.UI.AskingWindow;
using Game.UI.ScrollViewController;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Personal.LabelTab
{
    public class PlayersLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
        [SerializeField] private AskingWindow askingWindow;
        [SerializeField] private Sprite customLabelLogo;
        [Space]
        [SerializeField] private Image logo;
        [SerializeField] private Text labelName;
        [SerializeField] private Text exp;
        [SerializeField] private Text income;
        [SerializeField] private Text service;
        [SerializeField] private Button payServiceButton;
        [Space]
        [SerializeField] private int[] expToProductionLevelUp;
        [SerializeField] private Text production;
        [SerializeField] private ProgressBar productionBar;
        [SerializeField] private Button upProductionButton;
        [Space]
        [SerializeField] private int[] expToPrestigeLevelUp;
        [SerializeField] private PrestigeStars stars;
        [SerializeField] private ProgressBar prestigeBar;
        [SerializeField] private Button upPrestigeButton;
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        [Space]
        [SerializeField] private GameObject moneyReport;
        [SerializeField] private Text moneyReportIncome;
        [SerializeField] private Text moneyReportService;
        [SerializeField] private Text moneyReportFrozenWarning;
        [SerializeField] private Button moneyReportOkButton;
        [Space]
        [SerializeField] private Button disbandButton;
        
        private List<LabelMemberRow> _listItems = new();
        private LabelInfo _label;
        private int _income;
        private int _cost;

        private const int minLabelCost = 250_000;
        private const int maxStatValue = 5;
        private const int expStep = 100;
        
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
            
            TutorialManager.Instance.ShowTutorial("tutorial_players_label");
        }
        
        private void DisplayInfo(LabelInfo label)
        {
            _label = label;
            
            logo.sprite = customLabelLogo;
            labelName.text = label.Name;
            
            production.text = label.Production.Value.ToString();
            productionBar.SetValue(label.Production.Exp, expToProductionLevelUp[label.Production.Value]);
            
            upProductionButton.gameObject.SetActive(label.Production.Value < maxStatValue);
            upProductionButton.interactable = PlayerManager.Data.Exp >= expStep;
            
            float prestige = LabelsManager.GetLabelPrestige(label, expToPrestigeLevelUp);
            stars.Display(prestige);
            prestigeBar.SetValue(label.Prestige.Exp, expToPrestigeLevelUp[label.Prestige.Value]);
            
            upPrestigeButton.gameObject.SetActive(label.Prestige.Value < maxStatValue);
            upPrestigeButton.interactable = PlayerManager.Data.Exp >= expStep;
            
            exp.text = PlayerManager.Data.Exp.ToString();

            _income = LabelsManager.Instance.GetPlayersLabelIncome();
            income.text = _income.GetMoney();

            _cost = GetServiceCost();
            service.text = _cost.GetMoney();
            
            payServiceButton.gameObject.SetActive(label.IsFrozen);
            payServiceButton.interactable = PlayerManager.Data.Money >= _cost;
        }

        private void DisplayMembers(LabelInfo label)
        {
            var members = GetMembers(label.Name);
            
            for (var i = 0; i < members.Count; i++)
            {
                var info = members[i];
                
                var row = list.InstantiatedElement<LabelMemberRow>(template);
                row.Initialize(i+1, info);
                
                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }
        
        /// <summary>
        /// Returns all label members sorted desc by fans count
        /// </summary>
        private static List<RapperInfo> GetMembers(string labelName)
        {
            var members = RappersManager.Instance
                .GetAllRappers()
                .Where(e => e.Label == labelName)
                .ToList();
            
            members.Add(new RapperInfo
            {
                Name = PlayerManager.Data.Info.NickName,
                Fans = PlayerManager.Data.Fans / 1_000_000,
                Label = PlayerManager.Data.Label,
                IsPlayer = true
            });

            return members.OrderByDescending(r => r.Fans).ToList();
        }

        private int GetServiceCost()
        {
            float prestige = LabelsManager.GetLabelPrestige(_label, expToPrestigeLevelUp);
            return Mathf.Max((int) (prestige * 1_000_000), minLabelCost);
        }

        private void PayService()
        {
            SoundManager.Instance.PlayPayment();
            
            PlayerManager.Instance.AddMoney(-_cost);
            _label.IsFrozen = false;
            
            DisplayInfo(_label);
        }

        private void UpProduction()
        {
            SoundManager.Instance.PlayTrain();
            
            int level = _label.Production.Value;
            int expToUp = expToProductionLevelUp[level];

            int newExp = _label.Production.Exp + expStep;
            if (newExp >= expToUp)
            {
                SoundManager.Instance.PlayLevelUp();
                newExp -= expToUp;
                level += 1;
            }

            _label.Production.Value = level;
            _label.Production.Exp = newExp;
            
            PlayerManager.Instance.AddExp(-expStep);
            DisplayInfo(_label);
        }

        private void UpPrestige()
        {
            SoundManager.Instance.PlayTrain();
            
            int level = _label.Prestige.Value;
            int expToUp = expToPrestigeLevelUp[level];

            int newExp = _label.Prestige.Exp + expStep;
            if (newExp >= expToUp)
            {
                SoundManager.Instance.PlayLevelUp();
                newExp -= expToUp;
                level += 1;
            }

            _label.Prestige.Value = level;
            _label.Prestige.Exp = newExp;
            
            PlayerManager.Instance.AddExp(-expStep);
            DisplayInfo(_label);
        }
        
        private void DisbandLabel()
        {
            SoundManager.Instance.PlayClick();
            
            askingWindow.Show(
                LocalizationManager.Instance.Get("disband_label_question").ToUpper(),
                () => {
                    var members = GetMembers(_label.Name);
                    members.ForEach(e => e.Label = "");
                    PlayerManager.Data.Label = "";

                    LabelsManager.Instance.DisbandPlayersLabel();
            
                    labelTab.Reload();
                }
            );
        }

        public void ShowMoneyReport()
        {
            string incomeValue = (_label.IsFrozen ? 0 : _income).GetMoney();
            moneyReportIncome.text = LocalizationManager.Instance
                .GetFormat("label_monthly_income", incomeValue)
                .ToUpper();

            string serviceValue = (_label.IsFrozen ? 0 : _cost).GetMoney();
            moneyReportService.text = LocalizationManager.Instance
                .GetFormat("label_monthly_service", serviceValue)
                .ToUpper();
            
            moneyReportFrozenWarning.gameObject.SetActive(_label.IsFrozen);
            
            moneyReport.SetActive(true);
        }

        private void OnMoneyReportClose()
        {
            SoundManager.Instance.PlayClick();

            if (!_label.IsFrozen)
            {
                PlayerManager.Instance.AddMoney(_income);
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
            
            base.Close();
        }
    }
}