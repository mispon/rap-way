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
        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text ticketsSold;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;

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
            header.text = GetLocale("concert_result_header", concert.LocationName);
            ticketsSold.text = GetLocale("concert_result_sold", concert.TicketsSold.GetDisplay()).ToUpper();
            ticketCost.text = GetLocale("concert_result_cost", concert.TicketCost.GetMoney()).ToUpper();
            moneyIncome.text = $"+{concert.Income.GetMoney()}";
            expIncome.text = $"+{settings.ConcertRewardExp}";
        }

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private void SaveResult(ConcertInfo concert)
        {
            concert.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.AddMoney(concert.Income, settings.ConcertRewardExp);
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