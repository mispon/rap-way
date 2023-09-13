using Core;
using Game.Analyzers;
using Game.Pages.Eagler;
using Models.Game;
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
        [SerializeField] private Text listenAmount;
        [SerializeField] private Text duration;
        [SerializeField] private Text trackNameLabel;
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private Text chartInfo;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject hitBadge;

        [Header("Твитты фанатов")]
        [SerializeField] private EagleCard[] eagleCards;

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
        
            string trackName = track.Name;
            if (track.Feat != null)
            {
                trackName += $" feat. {track.Feat.Name}";
            }
            
            string fansIncomePrefix = track.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{fansIncomePrefix}{track.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{track.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.TrackRewardExp}";

            trackNameLabel.text = trackName;
            playerNameLabel.text = nickname;
            listenAmount.text = track.ListenAmount.GetDisplay();
            duration.text = GetDuration();
            
            hitBadge.SetActive(track.IsHit);
            
            chartInfo.text = track.ChartPosition > 0
                ? $"{track.ChartPosition}. {nickname} - {track.Name}"
                : GetLocale("track_result_no_chart");
            
            DisplayEagles(track.Quality);
        }

        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
            }
        }

        private string GetDuration()
        {
            int minutes = Random.Range(1, 6);
            int seconds = Random.Range(10, 60);
            return $"0{minutes}:{seconds}";
        }

        /// <summary>
        /// Сохраняет результаты трека
        /// </summary>
        private void SaveResult(TrackInfo track) 
        {
            track.Timestamp = TimeManager.Instance.Now.DateToString();
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