using Game.Analyzers;
using Models.Production;
using UnityEngine;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница результатов концерта
    /// </summary>
    public class ConcertResultPage : Page
    {
        [Header("Анализатор концерта")]
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

        private ConcertInfo _concert;
        
        /// <summary>
        /// Показывает результаты концерта
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            DisplayResult(concert);
            SaveResult(concert);
            Open();
        }

        /// <summary>
        /// Отображает результаты концерта 
        /// </summary>
        private void DisplayResult(ConcertInfo concert)
        {
            // todo
        }

        /// <summary>
        /// Сохраняет результаты концерта 
        /// </summary>
        private static void SaveResult(ConcertInfo concert)
        {
            PlayerManager.Instance.AddMoney(concert.Income);
            PlayerManager.Data.History.ConcertList.Add(concert);
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            concertAnalyzer.Analyze(_concert);
            // todo: Show concert cutscene
        }

        protected override void AfterPageClose()
        {
            _concert = null;
        }

        #endregion
    }
}