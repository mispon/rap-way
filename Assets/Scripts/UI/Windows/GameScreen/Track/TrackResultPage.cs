using System;
using Core.Context;
using Extensions;
using Game.Production;
using Game.Production.Analyzers;
using Game.SocialNetworks.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.UI;
using Models.Production;
using UI.Windows.GameScreen.SocialNetworks.Eagler;
using UnityEngine;
using UnityEngine.UI;
// using Firebase.Analytics;
using Random = UnityEngine.Random;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Track
{
    public class TrackResultPage : Page
    {
        [Header("Result")] [SerializeField] private Text listenAmount;

        [SerializeField] private Text       duration;
        [SerializeField] private Text       trackNameLabel;
        [SerializeField] private Text       playerNameLabel;
        [SerializeField] private Text       qualityLabel;
        [SerializeField] private Text       chartInfo;
        [SerializeField] private Text       fansIncome;
        [SerializeField] private Text       moneyIncome;
        [SerializeField] private Text       expIncome;
        [SerializeField] private GameObject hitBadge;

        [Header("Eagles")] [SerializeField]   private EaglerCard[]  eagleCards;
        [Header("Analyzer")] [SerializeField] private TrackAnalyzer trackAnalyzer;

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
            var nickname = PlayerAPI.Data.Info.NickName;

            var trackName = track.Name;
            if (track.Feat != null)
            {
                trackName += $" feat. {track.Feat.Name}";
            }

            var fansIncomePrefix = track.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text  = $"{fansIncomePrefix}{track.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{track.MoneyIncome.GetMoney()}";
            expIncome.text   = $"+{settings.Track.RewardExp}";

            trackNameLabel.text  = trackName;
            playerNameLabel.text = nickname;
            qualityLabel.text    = $"{Convert.ToInt32(track.Quality * 100)}%";
            listenAmount.text    = track.ListenAmount.GetDisplay();
            duration.text        = GetDuration();

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
            var minutes = Random.Range(1, 6);
            var seconds = Random.Range(10, 60);
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
                FansIncome  = track.FansIncome,
                Exp         = settings.Track.RewardExp
            });
        }

        protected override void AfterHide()
        {
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrackResultShown);
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());

            SaveResult(_track);
            _track = null;
        }
    }
}