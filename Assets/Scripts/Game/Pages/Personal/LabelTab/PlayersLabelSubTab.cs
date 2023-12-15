using System.Collections.Generic;
using System.Linq;
using Data;
using Game.Pages.Labels;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal.LabelTab
{
    public class PlayersLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
        [SerializeField] private Sprite customLabelLogo;
        [Space]
        [SerializeField] private Image logo;
        [SerializeField] private Text labelName;
        [SerializeField] private Text production;
        [SerializeField] private PrestigeStars stars;
        [SerializeField] private Button disbandButton;
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        
        private List<LabelMemberRow> _listItems = new();
        private LabelInfo _label;
        
        private void Start()
        {
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

            float prestige = LabelsManager.Instance.GetLabelPrestige(label);
            stars.Display(prestige);
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
        
        private void DisbandLabel()
        {
            var members = GetMembers(_label.Name);
            members.ForEach(e => e.Label = "");
            PlayerManager.Data.Label = "";

            LabelsManager.Instance.DisbandPlayersLabel();
            
            labelTab.Reload();
        }
    }
}