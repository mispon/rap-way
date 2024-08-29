using Core;
using Core.Ads;
using Core.Context;
using Enums;
using Core.Analytics;
using MessageBroker;
using MessageBroker.Messages.UI;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Personal
{
    internal enum PersonalTabType
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
        [SerializeField] private LabelTab.LabelTab labelTab;

        [Header("Реклама")]
        [SerializeField] private Button cashButton;

        private PersonalTabType _activeTab = PersonalTabType.None;
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
            var tab = ctx.Value<PersonalTabType>();
            switch (tab)
            {
                case PersonalTabType.House:
                    OpenHouseTab();
                    break;

                case PersonalTabType.Label:
                    OpenLabelTab();
                    break;

                case PersonalTabType.MoneyReport:
                    ShowLabelMoneyReport();
                    break;

                case PersonalTabType.Personal:
                case PersonalTabType.None:
                default:
                    OpenPersonalTab();
                    break;
            }

            base.Show(ctx);
        }

        private void OpenPersonalTab()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.PersonalPageOpened);

            if (!_isFirstOpen)
            {
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
            }
            _isFirstOpen = false;

            if (_activeTab != PersonalTabType.Personal)
            {
                _activeTab = PersonalTabType.Personal;
                UpdateTabs(personalTab, houseTab, labelTab);
                UpdateTabButtons(personalButton, houseButton, labelButton);
            }
        }

        private void OpenHouseTab()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.HousePageOpened);
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_activeTab != PersonalTabType.House)
            {
                _activeTab = PersonalTabType.House;
                UpdateTabs(houseTab, personalTab, labelTab);
                UpdateTabButtons(houseButton, personalButton, labelButton);
            }
        }

        private void OpenLabelTab()
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.LabelInfoPageOpened);
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_activeTab != PersonalTabType.Label)
            {
                _activeTab = PersonalTabType.Label;
                UpdateTabs(labelTab, personalTab, houseTab);
                UpdateTabButtons(labelButton, personalButton, houseButton);
            }
        }

        private void ShowLabelMoneyReport()
        {
            labelTab.Close();

            _activeTab = PersonalTabType.Label;
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
            _activeTab = PersonalTabType.None;
            _isFirstOpen = true;

            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
        }
    }
}