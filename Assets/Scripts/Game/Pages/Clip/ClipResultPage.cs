using Core;
using Game.Analyzers;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница результатов работы над клипом
    /// </summary>
    public class ClipResultPage : Page
    {
        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text viewsAmount;
        [SerializeField] private Text likesAndDislikes;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;

        [Header("Анализатор клипа")]
        [SerializeField] private ClipAnalyzer clipAnalyzer;

        private ClipInfo _clipInfo;
        
        /// <summary>
        /// Показывает результат работы над клипом
        /// </summary>
        public void Show(ClipInfo clip)
        {
            _clipInfo = clip;
            
            clipAnalyzer.Analyze(clip);
            DisplayResult(clip);
            Open();
        }

        /// <summary>t
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(ClipInfo clip)
        {
            DisplayEagles(clip.Quality);
            
            header.text = GetLocale("clip_result_header", ProductionManager.GetTrackName(clip.TrackId));
            viewsAmount.text = GetLocale("clip_result_views", clip.Views.GetDisplay());
            likesAndDislikes.text = GetLocale("clip_result_reaction", clip.Likes.GetDisplay(), clip.Dislikes.GetDisplay());
            string fansPrefix = clip.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{fansPrefix}{clip.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{clip.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.ClipRewardExp}";
        }
        
        private void DisplayEagles(float quality)
        {
            var fans = PlayerManager.Data.Fans;

            var eagles = EaglerManager.Instance.GenerateEagles(quality, fans);
            foreach (var eagle in eagles)
            {
                Debug.Log(eagle);
                // todo
            }
        }
        
        /// <summary>
        /// Сохраняет результаты клипа
        /// </summary>
        private void SaveResult(ClipInfo clip)
        {
            clip.Timestamp = TimeManager.Instance.Now.DateToString();
            PlayerManager.Instance.GiveReward(clip.FansIncome, clip.MoneyIncome, settings.ClipRewardExp);
            ProductionManager.AddClip(clip);
        }

        /// <summary>
        /// Выполняется перед закрытием страницы
        /// </summary>
        protected override void AfterPageClose()
        {
            SaveResult(_clipInfo);
            _clipInfo = null;
        }
    }
}