using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.Pages.Labels;
using Game.UI;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Personal.LabelTab
{
    public class PlayersLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
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
        [SerializeField] private Button disbandButton;
        
        private List<LabelMemberRow> _listItems = new();
        private LabelInfo _label;

        private const int minLabelCost = 250_000;
        private const int maxStatValue = 5;
        private const int expStep = 100;
        
        private void Start()
        {
            payServiceButton.onClick.AddListener(PayService);
            upProductionButton.onClick.AddListener(UpProduction);
            upPrestigeButton.onClick.AddListener(UpPrestige);
            disbandButton.onClick.AddListener(DisbandLabel);
        }

        public void Show(LabelInfo label)
        {
            DisplayInfo(label);
            DisplayMembers(label);
            
            base.Open();
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
            income.text = LabelsManager.Instance.GetPlayersLabelIncome().GetMoney();
            service.text = GetServiceCost().GetMoney();
            
            payServiceButton.gameObject.SetActive(label.IsFrozen);
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
            SoundManager.Instance.PlayClick();
            
            int cost = GetServiceCost();
            if (PlayerManager.Data.Money < cost)
            {
                // todo: show error
                return;
            }
            
            PlayerManager.Instance.AddMoney(-cost);
            GameManager.Instance.PlayerLabel.IsFrozen = false;
        }

        private void UpProduction()
        {
            SoundManager.Instance.PlayTrain();
            
            int level = _label.Production.Value;
            int expToUp = expToProductionLevelUp[level];

            int newExp = _label.Production.Exp + expStep;
            if (newExp >= expToUp)
            {
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
            // todo: ask are you sure?
            
            var members = GetMembers(_label.Name);
            members.ForEach(e => e.Label = "");
            PlayerManager.Data.Label = "";

            LabelsManager.Instance.DisbandPlayersLabel();
            
            labelTab.Reload();
        }
    }
}