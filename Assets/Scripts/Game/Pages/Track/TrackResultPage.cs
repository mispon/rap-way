using Core;
using Game.Analyzers;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

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
            string featInfo = track.Feat != null ? $" feat. {track.Feat.Name}" : string.Empty;
            header.text = $"Работа над треком <color=#01C6B8>\"{nickname} - {track.Name}{featInfo}\"</color> завершена";
            listenAmount.text = $"Композицию прослушали <color=#F6C326>{track.ListenAmount.GetDisplay()}</color> раз!";
            chartInfo.text = track.ChartPosition > 0
                ? $"Трек занял <color=#00F475>{track.ChartPosition}</color> позицию в чарте!"
                : "Трек не попал в топ чартов";
            fansIncome.text = $"+{track.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{track.MoneyIncome.GetMoney()}";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private void SaveResult(TrackInfo track) 
        {
            track.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.GiveReward(track.FansIncome, track.MoneyIncome, rewardExp);
            ProductionManager.AddTrack(track);
            
            if (track.Feat != null)
                ProductionManager.AddFeat(track.Feat);
        }
        
        /// <summary>
        /// Выполняется перед закрытием страницы
        /// </summary>
        protected override void AfterPageClose()
        {
            SaveResult(_trackInfo);
            _trackInfo = null;
        }
    }
}