using System.Linq;
using Core;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    public class NewRapperPage : Page
    {
        [SerializeField] private RappersGrid grid;
        [SerializeField] private int minRapperId;
        
        [Space]
        [Header("Ввод имени")]
        [SerializeField] private InputField nameInput;
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
        [Header("Кнопка создания")]
        [SerializeField] private Button createButton;

        private void Start()
        {
            createButton.onClick.AddListener(CreateButtonClick);
            
            vocabularyBtnLeft.onClick.AddListener(() => OnBntLeftClick(vocabularyValue));
            vocabularyBtnRight.onClick.AddListener(() => OnBntRightClick(vocabularyValue, 10));
            
            bitmakingBtnLeft.onClick.AddListener(() => OnBntLeftClick(bitmakingValue));
            bitmakingBtnRight.onClick.AddListener(() => OnBntRightClick(bitmakingValue, 10));
            
            managementBtnLeft.onClick.AddListener(() => OnBntLeftClick(managementValue));
            managementBtnRight.onClick.AddListener(() => OnBntRightClick(managementValue, 10));
            
            fansBtnLeft.onClick.AddListener(() => OnBntLeftClick(fansValue));
            fansBtnRight.onClick.AddListener(() => OnBntRightClick(fansValue, 50));
        }

        protected override void BeforePageOpen()
        {
            nameInput.text = "";
        }
        
        private static void OnBntLeftClick(Text value)
        {
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
            SoundManager.Instance.PlayClick();
            
            var nickname = nameInput.text;
            if (nickname.Length < 3)
            {
                HighlightError(nameInput);
                return;
            }

            int lastId = GameManager.Instance.CustomRappers.Count > 0 
                ? GameManager.Instance.CustomRappers.Max(r => r.Id)
                : minRapperId;
            
            var customRapper = new RapperInfo
            {
                Id = lastId + 1,
                Name = nickname,
                Vocobulary = int.Parse(vocabularyValue.text),
                Bitmaking = int.Parse(bitmakingValue.text),
                Management = int.Parse(managementValue.text),
                Fans = int.Parse(fansValue.text),
                IsCustom = true
            }; 
            
            GameManager.Instance.CustomRappers.Add(customRapper);
            grid.CreateItem(customRapper);
            
            Close();
        }
        
        private static void HighlightError(Component component)
        {
            var errorAnim = component.GetComponentInChildren<Animation>();
            errorAnim.Play();
        }
    }
}