#if UNITY_ANDROID
using System;
using System.Collections;
using CAS;
using Game;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;
#endif

using UnityEngine;

namespace Core.Ads
{
    public class CasAdsManager : Singleton<CasAdsManager>
    {
        [SerializeField] private int frequency = 5;

#if UNITY_ANDROID
        private IMediationManager _manager;

        private DateTime _lastShowTime = DateTime.Now;

        public void Start()
        {
            MobileAds.ValidateIntegration();
            MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = true;

            _manager = GetAdManager();
            _manager.OnRewardedAdCompleted += OnRewardedAdCompleted;
        }

        private static void OnRewardedAdCompleted()
        {
            int reward = Convert.ToInt32(PlayerAPI.Data.Money * 0.1f);
            reward = Math.Max(50, reward);

            SoundManager.Instance.PlaySound(UIActionType.Pay);
            MsgBroker.Instance.Publish(new ChangeMoneyMessage { Amount = reward });
        }

        private static IMediationManager GetAdManager()
        {
            // Configure MobileAds.settings before initialize
            return MobileAds.BuildManager()
                // Optional initialize listener
                .WithCompletionListener(config => { })
                .Build();
        }

        public void ShowInterstitial()
        {
            if (GameManager.Instance.LoadNoAds())
                return;

            var elapsed = DateTime.Now - _lastShowTime;
            if (elapsed.Minutes >= frequency)
            {
                bool adsLoaded = _manager.IsReadyAd(AdType.Interstitial);
                if (!adsLoaded)
                {
                    _manager.LoadAd(AdType.Interstitial);
                }

                _manager.ShowAd(AdType.Interstitial);
            }
        }

        public void ShowRewarded()
        {
            bool adsLoaded = _manager.IsReadyAd(AdType.Rewarded);
            if (!adsLoaded)
            {
                _manager.LoadAd(AdType.Rewarded);
            }

            _manager.ShowAd(AdType.Rewarded);
        }
#endif
    }
}