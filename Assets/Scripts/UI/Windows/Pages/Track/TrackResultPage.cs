using System;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Player;
using Game.Production;
using Game.Production.Analyzers;
using Game.Socials;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UI.Windows.Pages.Eagler;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.Pages.Track
{
    public class TrackResultPage : Page
    {
        [Header("Components")]
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text duration;
        [SerializeField] private Text trackNameLabel;
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private Text qualityLabel;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject hitBadge;

        [Header("Fans tweets")]
        [SerializeField] private EagleCard[] eagleCards;

        [Header("Track analyzer")]
        [SerializeField] private TrackAnalyzer trackAnalyzer;

        private TrackInfo _trackInfo;
        
        /// <summary>
        /// Показывает результат работы над треком
        /// </summary>
        public void Show(TrackInfo track)
        {
            _trackInfo = track;
            
            trackAnalyzer.Analyze(track);
            DisplayResult(track);
            Open();
        }

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(TrackInfo track)
        {
            var nickname = PlayerManager.Data.Info.NickName;
        
            string trackName = track.Name;
            if (track.Feat != null)
            {
                trackName += $" feat. {track.Feat.Name}";
            }
            
            string fansIncomePrefix = track.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{fansIncomePrefix}{track.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{track.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.TrackRewardExp}";

            trackNameLabel.text = trackName;
            playerNameLabel.text = nickname;
            qualityLabel.text = $"{Convert.ToInt32(track.Quality * 100)}%";
            listenAmount.text = track.ListenAmount.GetDisplay();
            duration.text = GetDuration();
            
            hitBadge.SetActive(track.IsHit);
            
            chartInfo.text = track.ChartPosition > 0
                ? $"{track.ChartPosition}. {nickname} - {track.Name}"
                : GetLocale("track_result_no_chart");
            
            DisplayEagles(track.Quality);
        }

        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

        private string GetDuration()
        {
            int minutes = Random.Range(1, 6);
            int seconds = Random.Range(10, 60);
            return $"0{minutes}:{seconds}";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private void SaveResult(TrackInfo track)
        {
            track.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddTrack(track);

            if (track.Feat != null)
            {
                ProductionManager.AddFeat(track.Feat);
            }
            
            MainMessageBroker.Instance.Publish(new ProductionRewardEvent
            {
                MoneyIncome = track.MoneyIncome,
                FansIncome = track.FansIncome,
                Exp = settings.TrackRewardExp
            });
        }
        
        /// <summary>
        /// Выполняется перед закрытием страницы
        /// </summary>
        protected override void AfterPageClose()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrackResultShown);
            
            UIMessageBroker.Instance.Publish(new TutorialWindowControlMessage());
            
            SaveResult(_trackInfo);
            _trackInfo = null;
        }
    }
}