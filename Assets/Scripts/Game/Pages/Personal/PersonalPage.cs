using Core;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Personal
{
    internal enum TabsType
    {
        None,
        Personal,
        House,
        Label
    }
    
    /// <summary>
    /// Персональная страница
    /// </summary>
    public class PersonalPage : Page
    {
        [Header("Tabs Buttons")]
        [SerializeField] private Button personalButton;
        [SerializeField] private Button houseButton;
        [SerializeField] private Button labelButton;
        [Space]
        [SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;
        
        [Header("Tabs")]
        [SerializeField] private PersonalTab.PersonalTab personalTab;
        [SerializeField] private HouseTab.HouseTab houseTab;
        [SerializeField] private LabelTab.LabelTab labelTab;

        [Header("Реклама")]
        [SerializeField] private Button cashButton;

        private TabsType _activeTab = TabsType.None;
        private bool _isFirstOpen = true;
        
        private void Start()
        {
            cashButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayClick();
                CasAdsManager.Instance.ShowRewarded();
            });
            
            personalButton.onClick.AddListener(OpenPersonalTab);
            houseButton.onClick.AddListener(OpenHouseTab);
            labelButton.onClick.AddListener(OpenLabelTab);
        }

        private void OpenPersonalTab()
        {
            if (!_isFirstOpen)
            {
                SoundManager.Instance.PlaySwitch();
            }
            _isFirstOpen = false;

            FirebaseAnalytics.LogEvent(FirebaseGameEvents.PersonalPageOpened);
            
            if (_activeTab != TabsType.Personal)
            {
                _activeTab = TabsType.Personal;
                UpdateTabs(personalTab, houseTab, labelTab);
                UpdateTabButtons(personalButton, houseButton, labelButton);
            }
        }
        
        private void OpenHouseTab()
        {
            SoundManager.Instance.PlaySwitch();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.HousePageOpened);

            if (_activeTab != TabsType.House)
            {
                _activeTab = TabsType.House;
                UpdateTabs(houseTab, personalTab, labelTab);
                UpdateTabButtons(houseButton, personalButton, labelButton);
            }
        }
        
        private void OpenLabelTab()
        {
            SoundManager.Instance.PlaySwitch();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelInfoPageOpened);

            if (_activeTab != TabsType.Label)
            {
                _activeTab = TabsType.Label;
                UpdateTabs(labelTab, personalTab, houseTab);
                UpdateTabButtons(labelButton, personalButton, houseButton);
            }
        }

        public void ShowLabelMoneyReport()
        {
            Open();
            labelTab.Close();
            
            _activeTab = TabsType.Label;
            UpdateTabs(labelTab, personalTab, houseTab);
            UpdateTabButtons(labelButton, personalButton, houseButton);

            labelTab.ShowMoneyReport();
        }
        
        protected override void BeforePageOpen()
        {
            OpenPersonalTab();
        }

        private static void UpdateTabs(params Tab[] tabs)
        {
            tabs[0].Open();
            tabs[1].Close();
            tabs[2].Close();
        }
        
        private void UpdateTabButtons(params Button[] buttons)
        {
            buttons[0].image.color = activeTabColor;
            buttons[1].image.color = inactiveTabColor;
            buttons[2].image.color = inactiveTabColor;
        }

        protected override void AfterPageClose()
        {
            _activeTab = TabsType.None;
            _isFirstOpen = true;
        }
    }
}