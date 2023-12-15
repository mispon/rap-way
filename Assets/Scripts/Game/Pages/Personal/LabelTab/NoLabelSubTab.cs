using Data;
using Models.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal.LabelTab
{
    public class NoLabelSubTab : Tab
    {
        [SerializeField] private LabelTab labelTab;

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
            
            int fans = PlayerManager.Data.Fans;
            bool canCreateLabel = fans >= fansRequirement;

            // noFansMessage.text = LocalizationManager.Instance.GetFormat("", fansRequirement.GetDisplay());
            noFansMessage.gameObject.SetActive(!canCreateLabel);
            createLabelGroup.SetActive(canCreateLabel);
            
            base.Open();
        }

        private void CreateLabel()
        {
            string labelName = labelNameInput.text;

            if (LabelsManager.Instance.IsNameAlreadyTaken(labelName))
            {
                // todo: show error
                return;
            }

            var label = new LabelInfo
            {
                Name = labelName,
                Prestige = new ExpValue(),
                Production = new ExpValue{Value = 1},
                IsPlayer = true,
            };
            LabelsManager.Instance.CreatePlayersLabel(label);
            
            PlayerManager.Data.Label = labelName;
            labelTab.Reload();
        }
    }
}