using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.Pages.Labels;
using Game.UI.AskingWindow;
using Game.UI.ScrollViewController;
using Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal.LabelTab
{
    public class GameLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
        [SerializeField] private AskingWindow askingWindow;
        [Space]
        [SerializeField] private Image logo;
        [SerializeField] private Text labelName;
        [SerializeField] private Text labelDesc;
        [SerializeField] private Text production;
        [SerializeField] private PrestigeStars stars;
        [SerializeField] private Button leaveButton;
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        
        private List<LabelMemberRow> _listItems = new();

        private void Start()
        {
            leaveButton.onClick.AddListener(LeaveLabel);
        }

        public void Show(LabelInfo label)
        {
            DisplayInfo(label);
            DisplayMembers(label);
            
            base.Open();
        }

        private void DisplayInfo(LabelInfo label)
        {
            logo.sprite = label.Logo;
            labelName.text = label.Name;
            labelDesc.text = label.Desc;
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

        private void LeaveLabel()
        {
            SoundManager.Instance.PlayClick();
            
            askingWindow.Show(
                LocalizationManager.Instance.Get("leave_label_question").ToUpper(),
                () => {
                    PlayerManager.Data.Label = "";
                    labelTab.Reload();
                }
            );
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