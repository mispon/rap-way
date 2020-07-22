using Core;
using Game.Analyzers;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

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
        
        [Header("Анализатор концерта")]
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

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
            header.text = $"Концерт в \"{concert.LocationName}\" завершен!";
            ticketsSold.text = $"ПРОДАНО {concert.TicketsSold} билетов";
            ticketCost.text = $"ЦЕНА БИЛЕТА: {concert.TicketCost}$";
            moneyIncome.text = $"ДОХОД: +{concert.Income}$";
        }

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private static void SaveResult(ConcertInfo concert)
        {
            PlayerManager.Instance.AddMoney(concert.Income);
            ProductionManager.AddConcert(concert);
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            // todo: Show concert cutscene
        }

        protected override void AfterPageClose()
        {
            SaveResult(_concert);
            _concert = null;
        }

        #endregion
    }
}