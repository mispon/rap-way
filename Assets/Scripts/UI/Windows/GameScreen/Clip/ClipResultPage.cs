using System;
using Core.Context;
using Extensions;
using Game.Production;
using Game.Production.Analyzers;
using Game.SocialNetworks.Eagler;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using UI.Windows.GameScreen.SocialNetworks.Eagler;
using UnityEngine;
using UnityEngine.UI;
// using Firebase.Analytics;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Clip
{
    public class ClipResultPage : Page
    {
        [Header("Result")] [SerializeField] private Text views;

        [SerializeField] private Text       likes;
        [SerializeField] private Text       dislikes;
        [SerializeField] private Text       clipNameLabel;
        [SerializeField] private Text       playerNameLabel;
        [SerializeField] private Text       qualityLabel;
        [SerializeField] private Text       fansIncome;
        [SerializeField] private Text       moneyIncome;
        [SerializeField] private Text       expIncome;
        [SerializeField] private GameObject hitBadge;

        [Header("Eagles")] [SerializeField]   private EaglerCard[] eagleCards;
        [Header("Analyzer")] [SerializeField] private ClipAnalyzer clipAnalyzer;

        private ClipInfo _clip;

        public override void Show(object ctx = null)
        {
            _clip = ctx.Value<ClipInfo>();

            clipAnalyzer.Analyze(_clip);
            DisplayResult(_clip);

            base.Show(ctx);
        }

        private void DisplayResult(ClipInfo clip)
        {
            var fansPrefix = clip.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text  = $"{fansPrefix}{clip.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{clip.MoneyIncome.GetMoney()}";
            expIncome.text   = $"+{settings.Clip.RewardExp}";

            clipNameLabel.text   = ProductionManager.GetTrackName(clip.TrackId);
            playerNameLabel.text = PlayerAPI.Data.Info.NickName;
            qualityLabel.text    = $"{Convert.ToInt32(clip.Quality * 100)}%";

            views.text    = clip.Views.GetDisplay();
            likes.text    = clip.Likes.GetDisplay();
            dislikes.text = clip.Dislikes.GetDisplay();

            hitBadge.SetActive(clip.IsHit);
            DisplayEagles(clip.Quality);
        }

        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

        private void SaveResult(ClipInfo clip)
        {
            clip.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddClip(clip);

            MsgBroker.Instance.Publish(new ProductionRewardMessage
            {
                MoneyIncome = clip.MoneyIncome,
                FansIncome  = clip.FansIncome,
                Exp         = settings.Clip.RewardExp
            });
        }

        protected override void AfterHide()
        {
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.ClipResultShown);

            SaveResult(_clip);
            _clip = null;
        }
    }
}