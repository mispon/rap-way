using System;
using System.Globalization;
using Core;
using Data;
using Game.Pages.Charts;
using Models.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Labels
{
    public class NewLabelPage : Page
    {
        [SerializeField] private LabelsPage labelsPage;
        [SerializeField] private ChartsPage chartsPage;
        
        [Space]
        [SerializeField] private InputField nameInput;
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

        protected override void BeforePageOpen()
        {
            nameInput.text = "";
            chartsPage.Hide();
        }

        protected override void AfterPageClose()
        {
            chartsPage.Show();
        }

        private void ProductionBntLeftClick()
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = int.Parse(productionValue.text);
            
            const int minValue = 1;
            if (current == minValue)
                return;

            current -= 1;
            productionValue.text = current.ToString();
        }
        
        private void ProductionBntRightClick()
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = int.Parse(productionValue.text);
         
            const int maxValue = 5;
            if (current == maxValue)
                return;

            current += 1;
            productionValue.text = current.ToString();
        }

        private void PrestigeBtnLeftClick()
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);
            
            const float minValue = 0.0f;
            if (Math.Abs(current - minValue) < 0.1)
                return;

            current -= 0.5f;
            prestigeValue.text = current.ToString(CultureInfo.InvariantCulture);
            
            stars.Display(current);
        }
        private void PrestigeBtnRightClick()
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);
            
            const float maxValue = 5.0f;
            if (Math.Abs(current - maxValue) < 0.1)
                return;

            current += 0.5f;
            prestigeValue.text = current.ToString(CultureInfo.InvariantCulture);
            
            stars.Display(current);
        }
        
        private void CreateButtonClick()
        {
            var labelName = nameInput.text;
            if (labelName.Length is < 3 or > 20)
            {
                HighlightError(nameInput);
                return;
            }

            if (LabelsManager.Instance.IsNameAlreadyTaken(labelName))
            {
                HighlightError(nameInput);
                return;
            }

            float prestige = float.Parse(prestigeValue.text, CultureInfo.InvariantCulture);
            
            var customLabel = new LabelInfo
            {
                Name = labelName,
                Production = new ExpValue{Value = int.Parse(productionValue.text)},
                Prestige = LabelsManager.Instance.FloatToExp(prestige),
                IsCustom = true
            };

            LabelsManager.Instance.AddCustom(customLabel);
            BackButtonClick();
        }

        private void BackButtonClick()
        {
            SoundManager.Instance.PlayClick();
            labelsPage.Open();
            Close();
        }
        
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }
    }
}