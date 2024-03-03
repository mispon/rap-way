using System;
using Core.Context;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Player;
using Game.Production;
using Game.Production.Analyzers;
using Game.Socials.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.UI;
using Models.Production;
using Sirenix.OdinInspector;
using UI.Windows.GameScreen.Eagler;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Windows.GameScreen.Track
{
    public class TrackResultPage : Page
    {
        [BoxGroup("Result"), SerializeField] private Text listenAmount;
        [BoxGroup("Result"), SerializeField] private Text duration;
        [BoxGroup("Result"), SerializeField] private Text trackNameLabel;
        [BoxGroup("Result"), SerializeField] private Text playerNameLabel;
        [BoxGroup("Result"), SerializeField] private Text qualityLabel;
        [BoxGroup("Result"), SerializeField] private Text chartInfo;
        [BoxGroup("Result"), SerializeField] private Text fansIncome;
        [BoxGroup("Result"), SerializeField] private Text moneyIncome;
        [BoxGroup("Result"), SerializeField] private Text expIncome;
        [BoxGroup("Result"), SerializeField] private GameObject hitBadge;

        [BoxGroup("Eagles"), SerializeField] private EagleCard[] eagleCards;
        [BoxGroup("Analyzer"), SerializeField] private TrackAnalyzer trackAnalyzer;

        private TrackInfo _track;
        
        public override void Show(object ctx = null)
        {
            _track = ctx.Value<TrackInfo>();
            
            trackAnalyzer.Analyze(_track);
            DisplayResult(_track);
            
            base.Show(ctx);
        }

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
            expIncome.text = $"+{settings.Track.RewardExp}";

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

        private static string GetDuration()
        {
            int minutes = Random.Range(1, 6);
            int seconds = Random.Range(10, 60);
            return $"0{minutes}:{seconds}";
        }

        private void SaveResult(TrackInfo track)
        {
            track.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddTrack(track);

            if (track.Feat != null)
            {
                ProductionManager.AddFeat(track.Feat);
            }
            
            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = track.MoneyIncome,
                FansIncome = track.FansIncome,
                Exp = settings.Track.RewardExp
            });
        }

        protected override void AfterHide()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrackResultShown);
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
            
            SaveResult(_track);
            _track = null;
        }
    }
}