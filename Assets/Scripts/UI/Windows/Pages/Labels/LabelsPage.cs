﻿using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Firebase.Analytics;
using Game.Labels;
using Game.Player;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.ScrollViewController;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Labels
{
    public class LabelsPage : Page
    {
        [Space]
        [SerializeField] private AskingWindow askingWindow;
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        [Space]
        [SerializeField] private LabelCard labelCard;
        [SerializeField] private Button addNewLabelButton;
        [SerializeField] private NewLabelPage newLabelPage;
        
        private List<LabelRow> _listItems = new();
        
        private void Start()
        {
            addNewLabelButton.onClick.AddListener(OpenNewLabelPage);
        }

        private void OpenNewLabelPage()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            newLabelPage.Open();
            Close();
        }

        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelsPageOpened);
            
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
        /// Returns all labels (internal and custom) sorted desc by score and prestige
        /// </summary>
        private static List<LabelInfo> GetAllLabels()
        {
            var labels = LabelsManager.Instance.GetAllLabels().ToList();
            
            if (LabelsManager.Instance.HasPlayerLabel)
            {
                labels.Add(LabelsManager.Instance.PlayerLabel);
            }

            return labels
                .OrderByDescending(e => e.Score)
                .ThenByDescending(e => LabelsManager.Instance.GetLabelPrestige(e))
                .ToList();
        }

        protected override void AfterPageOpen()
        {
            HintsManager.Instance.ShowHint("tutorial_labels");
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
            askingWindow.Show(
                GetLocale("delete_label_question"),
                () => {
                    LabelsManager.Instance.RemoveCustom(customLabel);
                    AfterPageClose();
                    BeforePageOpen();
                }
            );
        }
    }
}