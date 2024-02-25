using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Firebase.Analytics;
using Game.Rappers.Desc;
using ScriptableObjects;
using UI.Controls.Carousel;
using UI.Controls.Error;
using UI.Windows.Pages.Charts;
using UnityEngine;
using UnityEngine.UI;
using RappersAPI = Game.Rappers.RappersPackage;
using LabelsAPI = Game.Labels.LabelsPackage;

namespace UI.Windows.Pages.Rappers
{
    public class NewRapperPage : Page
    {
        [SerializeField] private RappersPage rappersPage;
        [SerializeField] private ChartsPage chartsPage;
        
        [Space]
        [Header("Ввод имени")]
        [SerializeField] private InputField nameInput;
        [SerializeField] private Carousel labelInput;
        [SerializeField] private GameError gameError;
        [Header("Ввод словарного запаса")]
        [SerializeField] private Button vocabularyBtnLeft;
        [SerializeField] private Button vocabularyBtnRight;
        [SerializeField] private Text vocabularyValue;
        [Header("Ввод битмейкинга")]
        [SerializeField] private Button bitmakingBtnLeft;
        [SerializeField] private Button bitmakingBtnRight;
        [SerializeField] private Text bitmakingValue;
        [Header("Ввод менеджмента")]
        [SerializeField] private Button managementBtnLeft;
        [SerializeField] private Button managementBtnRight;
        [SerializeField] private Text managementValue;
        [Header("Ввод фанатов")]
        [SerializeField] private Button fansBtnLeft;
        [SerializeField] private Button fansBtnRight;
        [SerializeField] private Text fansValue;
        
        [Space]
        [SerializeField] private Button createButton;
        [SerializeField] private Button backButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateButtonClick);
            backButton.onClick.AddListener(BackButtonClick);
            
            vocabularyBtnLeft.onClick.AddListener(() => OnBntLeftClick(vocabularyValue));
            vocabularyBtnRight.onClick.AddListener(() => OnBntRightClick(vocabularyValue, 10));
            
            bitmakingBtnLeft.onClick.AddListener(() => OnBntLeftClick(bitmakingValue));
            bitmakingBtnRight.onClick.AddListener(() => OnBntRightClick(bitmakingValue, 10));
            
            managementBtnLeft.onClick.AddListener(() => OnBntLeftClick(managementValue));
            managementBtnRight.onClick.AddListener(() => OnBntRightClick(managementValue, 10));
            
            fansBtnLeft.onClick.AddListener(() => OnBntLeftClick(fansValue));
            fansBtnRight.onClick.AddListener(() => OnBntRightClick(fansValue, 150));
        }

        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewRapperPageOpened);
            
            SetupLabelsCarousel();
            nameInput.text = "";
            chartsPage.Hide();
        }

        protected override void AfterPageClose()
        {
            chartsPage.ShowControls();
        }

        private void SetupLabelsCarousel()
        {
            var labels = LabelsAPI.Instance.GetAll().ToArray();
            var props = new List<CarouselProps>(labels.Length)
            {
                // empty value for no labels
                new() {Text = "None"}
            };

            foreach (var label in labels)
            {
                props.Add(new CarouselProps {Text = label.Name});
            }
            
            labelInput.Init(props.ToArray());
            labelInput.SetIndex(0);
        }

        private static void OnBntLeftClick(Text value)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            
            var current = int.Parse(value.text);
            if (current == 1)
            {
                return;
            }

            current -= 1;
            value.text = current.ToString();
        }
        
        private static void OnBntRightClick(Text value, int maxValue)
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            
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
            var nickname = nameInput.text;
            if (nickname.Length is < 3 or > 20)
            {
                var errorMsg = GetLocale("invalid_rapper_name_err");
                gameError.Show(errorMsg);
                HighlightError(nameInput);
                return;
            }
            
            if (RappersAPI.Instance.IsNameAlreadyTaken(nickname))
            {
                var errorMsg = GetLocale("rapper_name_exists_err");
                gameError.Show(errorMsg);
                HighlightError(nameInput);
                return;
            }

            int lastId = RappersAPI.Instance.MaxCustomRapperID();
            string label = labelInput.GetLabel();
            
            var customRapper = new RapperInfo
            {
                Id = lastId + 1,
                Name = nickname,
                Label = label != "None" ? label : "",
                Vocobulary = int.Parse(vocabularyValue.text),
                Bitmaking = int.Parse(bitmakingValue.text),
                Management = int.Parse(managementValue.text),
                Fans = int.Parse(fansValue.text),
                IsCustom = true
            };

            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewRapperCreated);
            RappersAPI.Instance.AddCustom(customRapper);
            BackButtonClick();
        }

        private void BackButtonClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            rappersPage.Open();
            Close();
        }
        
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }
    }
}