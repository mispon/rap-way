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
        
        [Space]
        [SerializeField] private Button createButton;
        [SerializeField] private Button backButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateButtonClick);
            backButton.onClick.AddListener(BackButtonClick);
            
            productionBtnLeft.onClick.AddListener(() => OnBntLeftClick(productionValue, 1));
            productionBtnRight.onClick.AddListener(() => OnBntRightClick(productionValue, 5));
            
            prestigeBtnLeft.onClick.AddListener(() => OnBntLeftClick(prestigeValue, 0));
            prestigeBtnRight.onClick.AddListener(() => OnBntRightClick(prestigeValue, 5));
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

        private static void OnBntLeftClick(Text value, int minValue)
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = int.Parse(value.text);
            if (current == minValue)
            {
                return;
            }

            current -= 1;
            value.text = current.ToString();
        }
        
        private static void OnBntRightClick(Text value, int maxValue)
        {
            SoundManager.Instance.PlaySwitch();
            
            var current = int.Parse(value.text);
            if (current == maxValue)
            {
                return;
            }

            current += 1;
            value.text = current.ToString();
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
            
            var customLabel = new LabelInfo
            {
                Name = labelName,
                Production = new ExpValue{Value = int.Parse(productionValue.text)},
                Prestige = new ExpValue{Value = int.Parse(prestigeValue.text)},
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