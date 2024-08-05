using Core.Context;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Production;
using Game.Production.Analyzers;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using MessageBroker.Messages.SocialNetworks;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Concert
{
    public class ConcertResultPage : Page
    {
        [Header("Result")]
        [SerializeField] private Text placeName;
        [SerializeField] private Text playerName;
        [SerializeField] private Text ticketsSold;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject soldOutBadge;

        [Header("Cutscene")]
        [SerializeField] private ConcertCutscenePage cutscenePage;

        [Header("Analyzer")]
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

        [Header("Images")]
        [SerializeField] private ImagesBank imagesBank;

        private ConcertInfo _concert;

        public override void Show(object ctx = null)
        {
            _concert = ctx.Value<ConcertInfo>();

            concertAnalyzer.Analyze(_concert);
            DisplayResult(_concert);

            base.Show(ctx);
        }

        private void DisplayResult(ConcertInfo concert)
        {
            placeName.text = concert.LocationName.ToUpper();
            playerName.text = PlayerAPI.Data.Info.NickName;

            moneyIncome.text = $"+{concert.Income.GetMoney()}";
            expIncome.text = $"+{settings.Concert.RewardExp}";

            ticketsSold.text = $"{concert.TicketsSold} / {concert.LocationCapacity}";
            ticketCost.text = concert.TicketCost.GetMoney();

            soldOutBadge.SetActive(concert.TicketsSold >= concert.LocationCapacity);
        }

        private static void SaveResult(ConcertInfo concert)
        {
            concert.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddConcert(concert);

            MsgBroker.Instance.Publish(new ConcertRewardMessage { MoneyIncome = concert.Income });
        }

        protected override void BeforeShow(object ctx = null)
        {
            cutscenePage.Show(_concert);
        }

        protected override void AfterHide()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ConcertResultShown);

            MsgBroker.Instance.Publish(new NewsMessage
            {
                Text = "news_player_finish_concert",
                TextArgs = new[] {
                    PlayerAPI.Data.Info.NickName,
                    _concert.LocationName
                },
                Sprite = imagesBank.NewsClip,
                Popularity = PlayerAPI.Data.Fans
            });

            SaveResult(_concert);
            _concert = null;
        }
    }
}