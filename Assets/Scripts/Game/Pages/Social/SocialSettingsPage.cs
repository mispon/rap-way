using System;
using Enums;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Models.Player;
using System.Linq;
using Game.Pages.Social.SocialStructs;
using Models.Info;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница настройки социальных действий
    /// </summary>
    public class SocialSettingsPage : Page
    {
        private const int MAX_ACTIONS = 6;
        
        [Header("Контролы")] 
        [SerializeField, ArrayElementTitle("Type")] private SocialSettingsUi[] uiElements;
        [SerializeField] private Text charityMoneyText;

        [Header("Управление ScrollRect")] 
        [SerializeField] private Scrollbar horScrollScrollbar;
        [SerializeField] private GameObject scrollLeftBtn;
        [SerializeField] private GameObject scrollRightBtn;
        
        [Header("Страницы")]
        [SerializeField] private SocialWorkingPage workingPage;
        
        [Header("Данные")]
        [SerializeField] private SocialData data;

        private SocialSettingsUi _lastSelected;
        private SocialSettingsUi UiElementsByType(SocialType type) => uiElements.First(uiE => uiE.Type == type);
        private SocialSettingsUi CharityUiElements => UiElementsByType(SocialType.Charity);
        
        private void Start()
        {
            var charitySlider = CharityUiElements.ExternalSlider;
            charitySlider.onValueChanged.AddListener(OnCharitySliderValueChange);
            OnCharitySliderValueChange(charitySlider.minValue);
            
            horScrollScrollbar.onValueChanged.AddListener(OnScrollSliderValueChange);
            OnScrollSliderValueChange(0);
            
            scrollLeftBtn.GetComponent<Button>().onClick.AddListener(() => { OnScrollButtonPressed(-1);});
            scrollRightBtn.GetComponent<Button>().onClick.AddListener(() => { OnScrollButtonPressed(+1);});

            var socialActivities = PlayerManager.Data.Socials.Values;
            for (int i = 0; i < uiElements.Length; i++)
            {
                var activity = socialActivities[i];
                var uiContainer = uiElements[i];
                
                uiElements[i].Btn.onClick.AddListener(
                    () => { DoSocial(activity, uiContainer);});
            }
        }

        /// <summary>
        /// При нажатии на кнопки перелистывания ScrollRect
        /// </summary>
        private void OnScrollButtonPressed(int value)
        {
            horScrollScrollbar.value += (value / (float) MAX_ACTIONS);
        }
        
        /// <summary>
        /// При изменении значения скроллбара ScrollRect
        /// </summary>
        private void OnScrollSliderValueChange(float value)
        {
            var newValue = value * MAX_ACTIONS;
            scrollLeftBtn.SetActive(newValue >= 1f);
            scrollRightBtn.SetActive(newValue <= 5f);
        }
        
        /// <summary>
        /// Активация кнопки социального действия
        /// </summary>
        /// <param name="type">Тип социального действия</param>
        /// <param name="value">Значение активации</param>
        public void SetActiveAction(SocialType type, bool value)
        {
            UiElementsByType(type).Btn.interactable = value;
        }
        
        /// <summary>
        /// Отображение значения слайдера благотворительности
        /// </summary>
        /// <param name="value"></param>
        private void OnCharitySliderValueChange(float value)
        {
            charityMoneyText.text = $"{value} $";
            //Запрещаем жертвовать 0 бабла.
            CharityUiElements.Btn.interactable &= Math.Abs(value) > 0.01f;
        }
        
        /// <summary>
        /// Открываем working-page с выбранным действием и его параметрами
        /// </summary>
        private void DoSocial(SocialActivity activity, SocialSettingsUi uiContainer)
        {
            _lastSelected = uiContainer;
            var settings = data.Socials.First(s => s.Type == uiContainer.Type);
            int charityMoney = uiContainer.ExternalSlider == null ? 0 : (int) uiContainer.ExternalSlider.value;
            
            var social = new SocialInfo
            {
                Activity = activity,
                Data = settings,
                ExternalText = uiContainer.ExternalTextField.text,
                CharityMoney = charityMoney
            };

            workingPage.ShowPage(social);
            
            SetActiveAction(settings.Type, false);
            Close();
        }

        #region PAGE CALLBACKS
        
        protected override void BeforePageOpen()
        {
            var money = PlayerManager.Data.Money;
            
            var socialActivities = PlayerManager.Data.Socials.Values;
            for (int i = 0; i < Mathf.Min(uiElements.Length, socialActivities.Length); i++)
            {
                var activity = socialActivities[i];
                uiElements[i].Btn.interactable = activity.IsActive;

                // В случае, если деактивация была запущена в предыдущую сессию,
                // то необходимо пересоздать callback на активацию кнопки по истечении времени
                if (!activity.IsActive && activity.ActivateAction == null)
                {
                    var index = i;
                    activity.ActivateAction = () => { SetActiveAction(uiElements[index].Type, true); };
                }
            }

            var charitySlider = CharityUiElements.ExternalSlider;
            charitySlider.minValue = (int)(data.minCharityPercentage * money);
            charitySlider.maxValue = money;
            charitySlider.interactable = (int)charitySlider.maxValue != 0;
            charitySlider.value = charitySlider.minValue;
        }

        protected override void BeforePageClose()
        {
            horScrollScrollbar.value = 0;
            CharityUiElements.ExternalSlider.value = 0;
            
            if(_lastSelected.ExternalTextField != null)
                _lastSelected.ExternalTextField.text = "";
            if(_lastSelected.ExternalSlider != null)
                _lastSelected.ExternalSlider.value = _lastSelected.ExternalSlider.minValue;
        }
        #endregion
    }
}


