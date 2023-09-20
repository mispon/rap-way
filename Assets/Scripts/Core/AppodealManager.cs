using System;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using Game;
using UnityEngine;
using Utils;

namespace Core
{
    public class AppodealManager : Singleton<AppodealManager>
    {
        private void Start()
        {
            const int adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo;
            const string appKey = "8799e5049fe3fec8966531205c563e62114a3abe25813f42";
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Appodeal.Initialize(appKey, adTypes);
        }
        private void OnInitializationFinished(object sender, SdkInitializedEventArgs e)
        {
            Debug.Log("Appodeal is loaded");
            ShowInterstitial();
            AppodealCallbacks.RewardedVideo.OnFinished += RewardedVideoFinished;
        }

        private void RewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs e)
        {
            var money = PlayerManager.Data.Money;
            var reward = Convert.ToInt32(money * 0.1);
            
            PlayerManager.Instance.AddMoney(reward);
        }
        
        public void ShowRewarded()
        {
            if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                Appodeal.Show(AppodealAdType.RewardedVideo);
            } else
            {
                Debug.LogError("ShowRewarded: not loaded");
            }
        }
        
        public void ShowInterstitial()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Interstitial))
            {
                Appodeal.Show(AppodealAdType.Interstitial);
            } else
            {
                Debug.LogError("ShowInterstitial: not loaded");
            }
        }
        
        public void ShowBanner()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Banner))
            {
                Appodeal.Show(AppodealShowStyle.BannerLeft);
            } else
            {
                Debug.LogError("ShowBanner: not loaded");
            }
        }
        
        public void ShowBannerRight()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Banner))
            {
                Appodeal.Show(AppodealShowStyle.BannerRight);
            }
        }
        
        public void HideBanner()
        {
            Appodeal.Hide(AppodealAdType.Banner);
        } 
    }
}