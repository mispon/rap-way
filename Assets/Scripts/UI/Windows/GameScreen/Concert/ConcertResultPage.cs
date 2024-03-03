using Core.Context;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Player;
using Game.Production;
using Game.Production.Analyzers;
using Game.Time;
using MessageBroker;
using MessageBroker.Messages.Production;
using Models.Production;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Concert
{
    public class ConcertResultPage : Page
    {
        [BoxGroup("Result"), SerializeField] private Text placeName;
        [BoxGroup("Result"), SerializeField] private Text playerName;
        [BoxGroup("Result"), SerializeField] private Text ticketsSold;
        [BoxGroup("Result"), SerializeField] private Text ticketCost;
        [BoxGroup("Result"), SerializeField] private Text moneyIncome;
        [BoxGroup("Result"), SerializeField] private Text expIncome;
        [BoxGroup("Result"), SerializeField] private GameObject soldOutBadge;
        
        [BoxGroup("Cutscene"), SerializeField] private ConcertCutscenePage cutscenePage;
        [BoxGroup("Analyzer"), SerializeField] private ConcertAnalyzer concertAnalyzer;
        
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
            playerName.text = PlayerManager.Data.Info.NickName;
            
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
            
            MsgBroker.Instance.Publish(new ConcertRewardMessage {MoneyIncome = concert.Income});
        }

        protected override void BeforeShow()
        {
            cutscenePage.Show(_concert);
        }

        protected override void AfterHide()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ConcertResultShown);
            
            SaveResult(_concert);
            _concert = null;
        }
    }
}