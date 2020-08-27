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
        [SerializeField] private int rewardExp;

        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text ticketsSold;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Text moneyIncome;
        
        [Header("Анализатор концерта")]
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

        [Header("Настройки катсцены")] 
        [SerializeField, Range(0.01f, 1f)] private float cutsceneShowChance;
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
            header.text = $"Концерт в \"{concert.LocationName}\" завершен!";
            ticketsSold.text = $"ПРОДАНО {concert.TicketsSold} билетов";
            ticketCost.text = $"ЦЕНА БИЛЕТА: {concert.TicketCost}$";
            moneyIncome.text = $"ДОХОД: +{concert.Income}$";
        }

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private void SaveResult(ConcertInfo concert)
        {
            PlayerManager.Instance.AddMoney(concert.Income, rewardExp);
            ProductionManager.AddConcert(concert);
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            if (Random.Range(0, 1) <= cutsceneShowChance)
                cutscenePage.Show(_concert);
        }

        protected override void AfterPageClose()
        {
            SaveResult(_concert);
            _concert = null;
        }

        #endregion
    }
}