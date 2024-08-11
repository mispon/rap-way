using Core;
using Core.Context;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.SocialNetworks
{
    internal enum SocialNetworksTabType
    {
        None,
        Email,
        News,
        Eagler
    }

    public class SocialNetworksPage : Page
    {
        [Header("Buttons")][SerializeField] private Button emailButton;
        [SerializeField] private Button newsButton;
        [SerializeField] private Button eaglerButton;

        [Space][SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;

        [Header("Tabs")][SerializeField] private Tab emailTab;
        [SerializeField] private Tab newsTab;
        [SerializeField] private Tab eaglerTab;

        private SocialNetworksTabType _activeTab = SocialNetworksTabType.None;
        private bool _isFirstOpen = true;

        private void Start()
        {
            emailButton.onClick.AddListener(OpenEmailTab);
            newsButton.onClick.AddListener(OpenNewsTab);
            eaglerButton.onClick.AddListener(OpenTwitterTab);
        }

        public override void Show(object ctx = null)
        {
            var tab = ctx.Value<SocialNetworksTabType>();
            switch (tab)
            {
                case SocialNetworksTabType.News:
                    OpenNewsTab();
                    break;

                case SocialNetworksTabType.Eagler:
                    OpenTwitterTab();
                    break;

                case SocialNetworksTabType.Email:
                case SocialNetworksTabType.None:
                default:
                    OpenEmailTab();
                    break;
            }

            base.Show(ctx);
        }

        private void OpenEmailTab()
        {
            if (!_isFirstOpen)
            {
                SoundManager.Instance.PlaySound(UIActionType.Switcher);
            }

            _isFirstOpen = false;

            if (_activeTab != SocialNetworksTabType.Email)
            {
                _activeTab = SocialNetworksTabType.Email;
                UpdateTabs(emailTab, newsTab, eaglerTab);
                UpdateTabButtons(emailButton, newsButton, eaglerButton);
            }
        }

        private void OpenNewsTab()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_activeTab != SocialNetworksTabType.News)
            {
                _activeTab = SocialNetworksTabType.News;
                UpdateTabs(newsTab, emailTab, eaglerTab);
                UpdateTabButtons(newsButton, emailButton, eaglerButton);
            }
        }

        private void OpenTwitterTab()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);

            if (_activeTab != SocialNetworksTabType.Eagler)
            {
                _activeTab = SocialNetworksTabType.Eagler;
                UpdateTabs(eaglerTab, emailTab, newsTab);
                UpdateTabButtons(eaglerButton, emailButton, newsButton);
            }
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
            _activeTab = SocialNetworksTabType.None;
            _isFirstOpen = true;

            emailTab.Close();
            newsTab.Close();
            eaglerTab.Close();
        }
    }
}