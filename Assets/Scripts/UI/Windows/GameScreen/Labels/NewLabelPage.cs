using System;
using System.Globalization;
using Core;
using Enums;
using Game.Labels.Desc;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Game;
using ScriptableObjects;
using UI.Controls.Error;
using UI.Enums;
using UI.Windows.GameScreen.Charts;
using UnityEngine;
using UnityEngine.UI;
using Core.Analytics;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.GameScreen.Labels
{
    public class NewLabelPage : Page
    {
        [SerializeField] private InputField nameInput;
        [SerializeField] private GameError gameError;

        [Header("Production inputs")]
        [SerializeField] private Button productionBtnLeft;
        [SerializeField] private Button productionBtnRight;
        [SerializeField] private Text productionValue;

        [Header("Prestige input")]
        [SerializeField] private Button prestigeBtnLeft;
        [SerializeField] private Button prestigeBtnRight;
        [SerializeField] private Text prestigeValue;
        [SerializeField] private PrestigeStars stars;

        [Space]
        [SerializeField] private Button createButton;
        [SerializeField] private Button backButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateButtonClick);
            backButton.onClick.AddListener(BackButtonClick);

            productionBtnLeft.onClick.AddListener(ProductionBntLeftClick);
            productionBtnRight.onClick.AddListener(ProductionBntRightClick);

            prestigeBtnLeft.onClick.AddListener(PrestigeBtnLeftClick);
            prestigeBtnRight.onClick.AddListener(PrestigeBtnRightClick);
        }

        protected override void BeforeShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.NewLabelPageOpened);
            nameInput.text = "";
        }

        private void ProductionBntLeftClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            var current = int.Parse(productionValue.text);

            const int minValue = 1;
            if (current == minValue)
            {
                return;
            }

            current--;
            productionValue.text = current.ToString();
        }

        private void ProductionBntRightClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            var current = int.Parse(productionValue.text);

            const int maxValue = 5;
            if (current == maxValue)
            {
                return;
            }

            current++;
            productionValue.text = current.ToString();
        }

        private void PrestigeBtnLeftClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            var current = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);

            const float minValue = 0.0f;
            if (Math.Abs(current - minValue) < 0.1)
            {
                return;
            }

            current -= 0.5f;
            prestigeValue.text = current.ToString(CultureInfo.InvariantCulture);

            stars.Display(current);
        }

        private void PrestigeBtnRightClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            var current = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);

            const float maxValue = 5.0f;
            if (Math.Abs(current - maxValue) < 0.1)
            {
                return;
            }

            current += 0.5f;
            prestigeValue.text = current.ToString(CultureInfo.InvariantCulture);

            stars.Display(current);
        }

        private void CreateButtonClick()
        {
            var labelName = nameInput.text;
            if (labelName.Length is < 3 or > 20)
            {
                var errorMsg = GetLocale("invalid_label_name_err");
                gameError.Show(errorMsg);
                HighlightError(nameInput);
                return;
            }

            if (LabelsAPI.Instance.IsNameAlreadyTaken(labelName))
            {
                var errorMsg = GetLocale("label_name_exists_err");
                gameError.Show(errorMsg);
                HighlightError(nameInput);
                return;
            }

            var prestige = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);

            var customLabel = new LabelInfo
            {
                Name = labelName,
                Desc = "custom_label_desc",
                Production = new ExpValue { Value = int.Parse(productionValue.text) },
                Prestige = LabelsAPI.Instance.PrestigeToExp(prestige),
                IsCustom = true
            };

            AnalyticsManager.LogEvent(FirebaseGameEvents.NewLabelCreated);
            LabelsAPI.Instance.AddCustom(customLabel);
            BackButtonClick();
        }

        private static void BackButtonClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage(WindowType.Charts, ChartsTabType.Labels));
        }

        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }
    }
}