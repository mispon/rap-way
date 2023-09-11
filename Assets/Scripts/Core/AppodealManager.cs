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
            const int adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo | AppodealAdType.Mrec;
            const string appKey = "";
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Appodeal.Initialize(appKey, adTypes);

            if (Appodeal.IsInitialized(AppodealAdType.RewardedVideo))
            {
                AppodealCallbacks.RewardedVideo.OnFinished += RewardedVideoFinished;
            }
        }
        private static void OnInitializationFinished(object sender, SdkInitializedEventArgs e)
        {
            Debug.Log("Appodeal loaded");
        }

        private void RewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs e)
        {
            var money = PlayerManager.Data.Money;
            var reward = (money / 100) * 10;
            
            PlayerManager.Instance.AddMoney(reward);
        }
        
        public void ShowRewarded()
        {
            if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
            {
                Appodeal.Show(AppodealAdType.RewardedVideo);
            }
        }
        
        public void ShowInterstitial()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Interstitial))
            {
                Appodeal.Show(AppodealAdType.Interstitial);
            }
        }
        
        public void ShowBanner()
        {
            if (Appodeal.IsLoaded(AppodealAdType.Banner))
            {
                Appodeal.Show(AppodealShowStyle.BannerLeft);
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