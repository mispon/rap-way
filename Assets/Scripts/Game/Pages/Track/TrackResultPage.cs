using Core;
using Game.Analyzers;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Track
{
    /// <summary>
    /// Страница результатов трека
    /// </summary>
    public class TrackResultPage : Page
    {
        [SerializeField] private int rewardExp;
        
        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;

        [Header("Анализатор трека")]
        [SerializeField] private TrackAnalyzer trackAnalyzer;

        private TrackInfo _trackInfo;
        
        /// <summary>
        /// Показывает результат работы над треком
        /// </summary>
        public void Show(TrackInfo track)
        {
            _trackInfo = track;
            
            trackAnalyzer.Analyze(track);
            DisplayResult(track);
            Open();
        }

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(TrackInfo track)
        {
            var nickname = PlayerManager.Data.Info.NickName;
            string featInfo = track.Feat != null ? $" feat. {track.Feat.Name}" : "";
            header.text = $"Завершена работа над треком: \"{nickname} - {track.Name}{featInfo}\"";
            listenAmount.text = $"Количество прослушиваний: {track.ListenAmount}";
            chartInfo.text = track.ChartPosition > 0
                ? $"Трек занял {track.ChartPosition}-ую позицию в чарте!"
                : "Трек не попал в топ чарта";
            fansIncome.text = $"ФАНАТЫ: +{track.FansIncome}";
            moneyIncome.text = $"ДЕНЬГИ: +{track.MoneyIncome}$";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private void SaveResult(TrackInfo track)
        {
            PlayerManager.Instance.GiveReward(track.FansIncome, track.MoneyIncome);
            PlayerManager.Instance.AddExp(rewardExp);
            ProductionManager.AddTrack(track);
        }

        #region PAGE EVENTS
        /// <summary>
        /// Выполняется перед открытием страницы
        /// </summary>
        protected override void BeforePageOpen()
        {
            // todo: запустить или не запускать случайное событие
        }
        
        /// <summary>
        /// Выполняется перед закрытием страницы
        /// </summary>
        protected override void AfterPageClose()
        {
            SaveResult(_trackInfo);
            _trackInfo = null;
        }
        #endregion
    }
}