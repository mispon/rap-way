﻿using Core;
using Core.Localization;
using Enums;
using Extensions;
using Core.Analytics;
using Game.Labels.Desc;
using Models.Game;
using ScriptableObjects;
using UI.Controls.Error;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Personal.LabelTab
{
    public class NoLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;
        [SerializeField] private GameError gameError;

        [Space]
        [SerializeField] private int fansRequirement = 10_000_000;
        [SerializeField] private Text noFansMessage;
        [SerializeField] private GameObject createLabelGroup;

        [Space]
        [SerializeField] private InputField labelNameInput;
        [SerializeField] private Button createButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateLabel);
        }

        public override void Open()
        {
            labelNameInput.text = "";

            int fans = PlayerAPI.Data.Fans;
            bool canCreateLabel = fans >= fansRequirement;

            createLabelGroup.SetActive(canCreateLabel);
            noFansMessage.gameObject.SetActive(!canCreateLabel);
            noFansMessage.text = LocalizationManager.Instance
                .GetFormat("label_fans_req_message", fansRequirement.GetDisplay())
                .ToUpper();

            base.Open();

            HintsManager.Instance.ShowHint("tutorial_no_labels", PersonalTabType.Label);
        }

        private void CreateLabel()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            string labelName = labelNameInput.text;

            if (labelName.Length is < 3 or > 25)
            {
                var errorMsg = LocalizationManager.Instance.Get("invalid_label_name_err");
                gameError.Show(errorMsg);
                HighlightError(labelNameInput);

                return;
            }

            if (LabelsAPI.Instance.IsNameAlreadyTaken(labelName))
            {
                var errorMsg = LocalizationManager.Instance.Get("label_name_exists_err");
                gameError.Show(errorMsg);
                HighlightError(labelNameInput);

                return;
            }

            AnalyticsManager.LogEvent(FirebaseGameEvents.CreatedOwnLabel);

            var label = new LabelInfo
            {
                Name = labelName,
                Prestige = new ExpValue(),
                Production = new ExpValue { Value = 1 },
                IsPlayer = true,
            };
            LabelsAPI.Instance.CreatePlayersLabel(label);

            PlayerAPI.Data.Label = labelName;
            labelTab.Reload();
        }

        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }
    }
}