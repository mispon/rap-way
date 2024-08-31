#if UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;
#endif

namespace Core.Ads
{
    public class YandexAdsManager : Singleton<YandexAdsManager>
    {
        [SerializeField] private int frequency = 5;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void ShowInterstitialAdv();

        [DllImport("__Internal")]
        private static extern void ShowRewardedAdv();

        private DateTime _lastShowTime = DateTime.Now;

        public void ShowInterstitial()
        {
            var elapsed = DateTime.Now - _lastShowTime;

            if (elapsed.Minutes >= frequency)
            {
                ShowInterstitialAdv();
                _lastShowTime = DateTime.Now;
            }
        }

        public void ShowRewarded()
        {
            ShowRewardedAdv();
        }

        public void OnRewardedAdCompleted()
        {
            int reward = Convert.ToInt32(PlayerAPI.Data.Money * 0.1f);
            reward = Math.Max(50, reward);

            SoundManager.Instance.PlaySound(UIActionType.Pay);
            MsgBroker.Instance.Publish(new ChangeMoneyMessage { Amount = reward });
        }
#endif
    }
}