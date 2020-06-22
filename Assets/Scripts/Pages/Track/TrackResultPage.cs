using Analyzers;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Pages.Track
{
    /// <summary>
    /// Страница резултатов трека
    /// </summary>
    public class TrackResultPage : Page
    {
        [Header("Компоменты")]
        [SerializeField] private Text header;

        [Header("Анализатор трека")]
        [SerializeField] private TrackAnalyzer trackAnalyzer;

        /// <summary>
        /// Показывает результат 
        /// </summary>
        public void Show(TrackInfo track)
        {
            trackAnalyzer.Analyze(track);
            DisplayResult(track);
            GiveReward(track);
            Open();
        }

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(TrackInfo track)
        {
            header.text = $"Работа над треком \"{track.Name}\" завершена!";
        }

        /// <summary>
        /// Выдает награду игроку 
        /// </summary>
        private void GiveReward(TrackInfo track)
        {
            
        }

        /// <summary>
        /// Выполняется перед открытием страницы
        /// </summary>
        protected override void BeforePageOpen()
        {
            // todo: запустить или не запускать случайное событие
        }
    }
}