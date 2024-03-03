using Core;
using Core.Ads;
using Core.Context;
using Enums;
using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Personal
{
    internal enum TabsType
    {
        None,
        Personal,
        House,
        Label,
        MoneyReport
    }

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
        [SerializeField] private global::UI.Windows.GameScreen.Personal.LabelTab.LabelTab labelTab;

        [Header("Реклама")]
        [SerializeField] private Button cashButton;

        private TabsType _activeTab = TabsType.None;
        private bool _isFirstOpen = true;
        
        private void Start()
        {
            cashButton.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(UIActionType.Click);
                CasAdsManager.Instance.ShowRewarded();
            });
            
            personalButton.onClick.AddListener(OpenPersonalTab);
            houseButton.onClick.AddListener(OpenHouseTab);
            labelButton.onClick.AddListener(OpenLabelTab);
        }
        
        public override void Show(object ctx = null)
        {
            var tab = ctx.Value<TabsType>();
            switch (tab)
            {
                case TabsType.House:
                    OpenHouseTab();
                    break;
                
                case TabsType.Label:
                    OpenLabelTab();
                    break;
                
                case TabsType.MoneyReport:
                    ShowLabelMoneyReport();
                    break;
                
                case TabsType.Personal:
                case TabsType.None:
                default:
                    OpenPersonalTab();
                    break;
            }
            
            base.Show(ctx);
        }

        private void OpenPersonalTab()
        {
            if (!_isFirstOpen)
            {
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
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
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
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
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.LabelInfoPageOpened);

            if (_activeTab != TabsType.Label)
            {
                _activeTab = TabsType.Label;
                UpdateTabs(labelTab, personalTab, houseTab);
                UpdateTabButtons(labelButton, personalButton, houseButton);
            }
        }

        private void ShowLabelMoneyReport()
        {
            labelTab.Close();
            
            _activeTab = TabsType.Label;
            UpdateTabs(labelTab, personalTab, houseTab);
            UpdateTabButtons(labelButton, personalButton, houseButton);

            labelTab.ShowMoneyReport();
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

        protected override void AfterHide()
        {
            _activeTab = TabsType.None;
            _isFirstOpen = true;
            
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
        }
    }
}