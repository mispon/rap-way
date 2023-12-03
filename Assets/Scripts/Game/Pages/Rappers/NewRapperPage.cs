using System.Linq;
using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    public class NewRapperPage : Page
    {
        [SerializeField] private RappersData rappersData;
        [SerializeField] private RappersPage rappersPage;

        [Space]
        [SerializeField] private GameObject[] mainPageControls;
        
        [Space]
        [Header("Ввод имени")]
        [SerializeField] private InputField nameInput;
        [SerializeField] private InputField labelInput;
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
            nameInput.text = "";
            labelInput.text = "";
            
            foreach (var go in mainPageControls)
            {
                go.SetActive(false);
            }
        }

        protected override void AfterPageClose()
        {
            foreach (var go in mainPageControls)
            {
                go.SetActive(true);
            }
        }

        private static void OnBntLeftClick(Text value)
        {
            SoundManager.Instance.PlaySwitch();
            
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
            var nickname = nameInput.text;
            if (nickname.Length is < 3 or > 20)
            {
                HighlightError(nameInput);
                return;
            }
            
            var label = labelInput.text;
            if (label.Length is < 3 or > 20)
            {
                HighlightError(nameInput);
                return;
            }

            int lastId = GameManager.Instance.CustomRappers.Count > 0 
                ? GameManager.Instance.CustomRappers.Max(r => r.Id)
                : rappersData.Rappers.Max(r => r.Id);
            
            var customRapper = new RapperInfo
            {
                Id = lastId + 1,
                Name = nickname,
                Label = label,
                Vocobulary = int.Parse(vocabularyValue.text),
                Bitmaking = int.Parse(bitmakingValue.text),
                Management = int.Parse(managementValue.text),
                Fans = int.Parse(fansValue.text),
                IsCustom = true
            }; 
            
            GameManager.Instance.CustomRappers.Add(customRapper);
            BackButtonClick();
        }

        private void BackButtonClick()
        {
            SoundManager.Instance.PlayClick();
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