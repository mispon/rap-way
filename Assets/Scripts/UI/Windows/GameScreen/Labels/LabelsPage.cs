using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Firebase.Analytics;
using Game.Labels.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UI.Controls.Ask;
using UI.Controls.ScrollViewController;
using UI.Enums;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Labels
{
    public class LabelsPage : Page
    {
        [SerializeField] private AskingWindow askingWindow;
        [SerializeField] private ScrollViewController list;
        [SerializeField] private GameObject template;
        [Space]
        [SerializeField] private LabelCard labelCard;
        [SerializeField] private Button addNewLabelButton;

        private readonly List<LabelRow> _listItems = new();
        
        private void Start()
        {
            addNewLabelButton.onClick.AddListener(OpenNewLabelPage);
        }

        private static void OpenNewLabelPage()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.NewLabel));
        }

        protected override void BeforeShow()
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

        private static List<LabelInfo> GetAllLabels()
        {
            var labels = LabelsAPI.Instance.GetAll().ToList();
            
            if (!LabelsAPI.Instance.IsPlayerLabelEmpty)
            {
                labels.Add(LabelsAPI.Instance.PlayerLabel);
            }

            return labels
                .OrderByDescending(e => e.Score)
                .ThenByDescending(e => LabelsAPI.Instance.GetPrestige(e))
                .ToList();
        }

        protected override void AfterShow()
        {
            HintsManager.Instance.ShowHint("tutorial_labels");
        }
        
        protected override void AfterHide()
        {
            labelCard.onDelete -= HandleLabelDelete;
            
            foreach (var row in _listItems)
            {
                Destroy(row.gameObject);
            }
            
            _listItems.Clear();
        }
        
        private void HandleLabelDelete(LabelInfo customLabel)
        {
            askingWindow.Show(
                GetLocale("delete_label_question"),
                () => {
                    LabelsAPI.Instance.RemoveCustom(customLabel);
                    AfterHide();
                    BeforeShow();
                }
            );
        }
    }
}