using Core;
using Game.Analyzers;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница результатов концерта
    /// </summary>
    public class ConcertResultPage : Page
    {
        [SerializeField] private int rewardExp;

        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text ticketsSold;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Text moneyIncome;
        
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
            header.text = $"Концерт в <color=#01C6B8>\"{concert.LocationName}\"</color> завершен!";
            ticketsSold.text = $"ПРОДАНО <color=#F6C326>{concert.TicketsSold.GetDisplay()}</color> билетов";
            ticketCost.text = $"ЦЕНА БИЛЕТА: <color=#00F475>{concert.TicketCost.GetDisplay()}$</color>";
            moneyIncome.text = $"+{concert.Income.GetDisplay()}$";
        }

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private void SaveResult(ConcertInfo concert)
        {
            concert.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.AddMoney(concert.Income, rewardExp);
            ProductionManager.AddConcert(concert);
        }

        protected override void BeforePageOpen()
        {
            cutscenePage.Show(_concert);
        }

        protected override void AfterPageClose()
        {
            SaveResult(_concert);
            _concert = null;
        }
    }
}