using System;
using System.Collections;
using CAS;
using Game;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace Core.Ads
{
    public class CasAdsManager : Singleton<CasAdsManager>
    {
        private IMediationManager _manager;
        
        public void Start()
        {
            MobileAds.ValidateIntegration();
            MobileAds.settings.allowInterstitialAdsWhenVideoCostAreLower = true;
            
            _manager = GetAdManager();
            _manager.OnRewardedAdCompleted += OnRewardedAdCompleted;
            
            StartCoroutine(ShowInterstitialLoop());
        }

        private static void OnRewardedAdCompleted()
        {
            int reward = Convert.ToInt32(PlayerAPI.Data.Money * 0.1f);
            reward = Math.Max(50, reward);
            
            SoundManager.Instance.PlaySound(UIActionType.Pay);
            MsgBroker.Instance.Publish(new ChangeMoneyMessage {Amount = reward});
        }

        private static IMediationManager GetAdManager()
        {
            // Configure MobileAds.settings before initialize
            return MobileAds.BuildManager()
                // Optional initialize listener
                .WithCompletionListener(config => { })
                .Build();
        }

        private void ShowInterstitial()
        {
            if (GameManager.Instance.LoadNoAds())
                return;
            
            bool adLoaded = _manager.IsReadyAd(AdType.Interstitial);
            if (!adLoaded)
            {
                _manager.LoadAd(AdType.Interstitial);
            }
            
            _manager.ShowAd(AdType.Interstitial);
        }
        
        public void ShowRewarded()
        {
            _manager.ShowAd(AdType.Rewarded);
        }

        private IEnumerator ShowInterstitialLoop()
        {
            while (true)
            {
                if (Application.isPlaying || Application.isFocused)
                {
                    yield return new WaitForSeconds(300);
                    ShowInterstitial();
                } else
                {
                    yield return new WaitForSeconds(10);
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}