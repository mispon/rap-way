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
        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;

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
            header.text = GetLocale("track_result_header", nickname, track.Name, featInfo);
            listenAmount.text = GetLocale("track_result_listens", track.ListenAmount.GetDisplay());
            chartInfo.text = track.ChartPosition > 0
                ? GetLocale("track_result_chart_pos", track.ChartPosition)
                : GetLocale("track_result_no_chart");
            string fansIncomePrefix = track.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{fansIncomePrefix}{track.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{track.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.TrackRewardExp}";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private void SaveResult(TrackInfo track) 
        {
            track.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.GiveReward(track.FansIncome, track.MoneyIncome, settings.TrackRewardExp);
            ProductionManager.AddTrack(track);

            if (track.Feat != null)
            {
                ProductionManager.AddFeat(track.Feat);
            }
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