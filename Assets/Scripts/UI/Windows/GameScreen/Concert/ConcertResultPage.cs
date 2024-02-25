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
using UI.Windows.GameScreen;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Concert
{
    /// <summary>
    /// Страница результатов концерта
    /// </summary>
    public class ConcertResultPage : Page
    {
        [Header("Компоменты")]
        [SerializeField] private Text placeName;
        [SerializeField] private Text playerName;
        [SerializeField] private Text ticketsSold;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject soldOutBadge;

        [Header("Анализатор концерта")]
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

        [Header("Катсцена")] 
        [SerializeField] private ConcertCutscenePage cutscenePage;
        
        private ConcertInfo _concert;
        
        /// <summary>
        /// Показывает результаты концерта
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            
            concertAnalyzer.Analyze(concert);
            DisplayResult(concert);
            Open();
        }

        /// <summary>
        /// Отображает результаты концерта 
        /// </summary>
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

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private void SaveResult(ConcertInfo concert)
        {
            concert.Timestamp = TimeManager.Instance.Now.DateToString();
            ProductionManager.AddConcert(concert);
            
            MsgBroker.Instance.Publish(new ConcertRewardMessage {MoneyIncome = concert.Income});
        }

        protected override void BeforePageOpen()
        {
            cutscenePage.Show(_concert);
        }

        protected override void AfterPageClose()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ConcertResultShown);
            
            SaveResult(_concert);
            _concert = null;
        }
    }
}