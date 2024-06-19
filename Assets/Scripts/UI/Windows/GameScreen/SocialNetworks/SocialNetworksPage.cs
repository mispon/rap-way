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
        Twitter
    }

    public class SocialNetworksPage : Page
    {
        [Header("Tab buttons")]
        [SerializeField] private Button emailButton;
        [SerializeField] private Button newsButton;
        [SerializeField] private Button twitterButton;
        [Space]
        [SerializeField] private Color activeTabColor;
        [SerializeField] private Color inactiveTabColor;
        
        [Header("Tabs")]
        [SerializeField] private Tab emailTab;
        [SerializeField] private Tab newsTab;
        [SerializeField] private Tab twitterTab;
        
        private SocialNetworksTabType _activeTab = SocialNetworksTabType.None;
        private bool _isFirstOpen = true;

        private void Start()
        {
            emailButton.onClick.AddListener(OpenEmailTab);
            newsButton.onClick.AddListener(OpenNewsTab);
            twitterButton.onClick.AddListener(OpenTwitterTab);
        }

        public override void Show(object ctx = null)
        {
            var tab = ctx.Value<SocialNetworksTabType>();
            switch (tab)
            {
                case SocialNetworksTabType.News:
                    OpenNewsTab();
                    break;

                case SocialNetworksTabType.Twitter:
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
                UpdateTabs(emailTab, newsTab, twitterTab);
                UpdateTabButtons(emailButton, newsButton, twitterButton);
            }
        }
        
        private void OpenNewsTab()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
    
            if (_activeTab != SocialNetworksTabType.News)
            {
                _activeTab = SocialNetworksTabType.News;
                UpdateTabs(newsTab, emailTab, twitterTab);
                UpdateTabButtons(newsButton, emailButton, twitterButton);
            }
        }
        
        private void OpenTwitterTab()
        {
            SoundManager.Instance.PlaySound(UIActionType.Switcher);
    
            if (_activeTab != SocialNetworksTabType.Twitter)
            {
                _activeTab = SocialNetworksTabType.Twitter;
                UpdateTabs(twitterTab, emailTab, newsTab);
                UpdateTabButtons(twitterButton, emailButton, newsButton);
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
        }
    }
}