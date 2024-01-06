using Core;
using Data;
using Firebase.Analytics;
using Game.Pages.Social.Tabs;
using UnityEngine;
using Models.Info;
using Utils.Carousel;

namespace Game.Pages.Social
{
    /// <summary>
    /// Страница настройки социальных действий
    /// </summary>
    public class SocialSettingsPage : Page
    {
        [Header("Вкладки")]
        [SerializeField] private Carousel tabsCarousel;
        [SerializeField] private BaseSocialsTab[] tabs;
        
        [Header("Страница работы")]
        [SerializeField] private SocialWorkingPage workingPage;

        private void Start()
        {
            tabsCarousel.onChange += OnTabChanged;
            foreach (var tab in tabs)
            {
                tab.onStartSocial += StartSocial;
            }
        }

        protected override void AfterPageOpen()
        {
            TutorialManager.Instance.ShowTutorial("tutorial_socials");
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.SocialsPageOpened);
        }

        /// <summary>
        /// Обработчик изменения вкладки 
        /// </summary>
        private void OnTabChanged(int index)
        {
            for (var i = 0; i < tabs.Length; i++)
            {
                tabs[i].SetVisible(i == index);
            }
        }
 
        /// <summary>
        /// Запускает работу по социальному действию 
        /// </summary>
        private void StartSocial(SocialInfo info)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            workingPage.StartWork(info);
            Close();
        }

        protected override void BeforePageOpen()
        {
            foreach (var tab in tabs)
            {
                tab.Refresh();
            }
        }

        private void OnDestroy()
        {
            tabsCarousel.onChange -= OnTabChanged;
            foreach (var tab in tabs)
            {
                tab.onStartSocial -= StartSocial;
            }
        }
    }
}