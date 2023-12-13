using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.UI.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Labels
{
    public class LabelsPage : Page
    {
        [Space]
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        [Space]
        [SerializeField] private LabelCard labelCard;
        [SerializeField] private Button addNewLabelButton;
        [SerializeField] private NewLabelPage newLabelPage;
        
        private List<LabelRow> _listItems = new();
        
        private void Start()
        {
            addNewLabelButton.onClick.AddListener(OpenNewRapperPage);
        }

        private void OpenNewRapperPage()
        {
            SoundManager.Instance.PlayClick();
            newLabelPage.Open();
            Close();
        }

        protected override void BeforePageOpen()
        {
            labelCard.onDelete += HandleLabelDelete;
            
            var labels = GetAllLabels();
            for (var i = 0; i < labels.Count; i++)
            {
                var info = labels[i];
                
                var row = list.InstantiatedElement<LabelRow>(template);
                row.Initialize(i+1, info);
                
                _listItems.Add(row);
            }

            list.RepositionElements(_listItems);
        }

        /// <summary>
        /// Returns all rappers (internal and custom) sorted desc by fans count
        /// </summary>
        private static List<LabelInfo> GetAllLabels()
        {
            var allRappers = LabelsManager.Instance.GetAllLabels().ToList();
            
            allRappers.Add(GameManager.Instance.PlayerLabel);

            return allRappers
                .OrderByDescending(e => e.Score)
                .ThenBy(e => LabelsManager.Instance.GetLabelPrestige(e))
                .ToList();
        }

        protected override void AfterPageOpen()
        {
            // TutorialManager.Instance.ShowTutorial("tutorial_labels");
        }
        
        protected override void AfterPageClose()
        {
            labelCard.onDelete -= HandleLabelDelete;
            
            foreach (var row in _listItems)
            {
                Destroy(row.gameObject);
            }
            
            _listItems.Clear();
        }

        /// <summary>
        /// Process custom rapper remove
        /// </summary>
        private void HandleLabelDelete(LabelInfo customLabel)
        {
            LabelsManager.Instance.RemoveCustom(customLabel);
            AfterPageClose();
            BeforePageOpen();
        }
    }
}