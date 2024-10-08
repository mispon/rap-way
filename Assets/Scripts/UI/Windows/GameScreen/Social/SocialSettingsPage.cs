﻿using Core;
using Enums;
using Core.Analytics;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using UI.Controls.Carousel;
using UI.Enums;
using UI.Windows.GameScreen.Social.Tabs;
using UI.Windows.Tutorial;
using UnityEngine;

namespace UI.Windows.GameScreen.Social
{
    public class SocialSettingsPage : Page
    {
        [SerializeField] private Carousel tabsCarousel;
        [SerializeField] private BaseSocialsTab[] tabs;

        private void Start()
        {
            tabsCarousel.onChange += OnTabChanged;
            foreach (var tab in tabs)
            {
                tab.onStartSocial += StartSocial;
            }
        }

        private void OnTabChanged(int index)
        {
            for (var i = 0; i < tabs.Length; i++)
            {
                tabs[i].SetVisible(i == index);
            }
        }

        private static void StartSocial(SocialInfo info)
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.SocialsWork,
                Context = info
            });
        }

        protected override void BeforeShow(object ctx = null)
        {
            foreach (var tab in tabs)
            {
                tab.Refresh();
            }
        }

        protected override void AfterShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.SocialsPageOpened);
            HintsManager.Instance.ShowHint("tutorial_socials");
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