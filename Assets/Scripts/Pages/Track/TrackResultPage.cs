using Analyzers;
using Core;
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
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;

        [Header("Анализатор трека")]
        [SerializeField] private TrackAnalyzer trackAnalyzer;

        /// <summary>
        /// Показывает результат 
        /// </summary>
        public void Show(TrackInfo track)
        {
            trackAnalyzer.Analyze(track);
            DisplayResult(track);
            SaveResult(track);
            Open();
        }

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(TrackInfo track)
        {
            header.text = $"Работа над треком \"{track.Name}\" завершена!";
            listenAmount.text = $"Количество прослушиваний: {track.ListenAmount}";
            chartInfo.text = track.ChartPosition > 0
                ? $"Трек занял {track.ChartPosition}-ую позицию в чарте!"
                : "Трек не попал в топ чарта";
            fansIncome.text = $"ФАНАТЫ: +{track.FansIncome}";
            moneyIncome.text = $"ДЕНЬГИ: +{track.MoneyIncome}";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private static void SaveResult(TrackInfo track)
        {
            PlayerManager.Instance.GiveReward(track.FansIncome, track.MoneyIncome);
            PlayerManager.PlayerData.History.TrackList.Add(track);
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