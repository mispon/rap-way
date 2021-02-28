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
        [SerializeField] private int rewardExp;

        [Header("Компоменты")]
        [SerializeField] private Text header;
        [SerializeField] private Text viewsAmount;
        [SerializeField] private Text likesAndDislikes;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;

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

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(ClipInfo clip)
        {
            header.text = GetLocale("clip_result_header", ProductionManager.GetTrackName(clip.TrackId));
            viewsAmount.text = GetLocale("clip_result_views", clip.Views.GetDisplay());
            likesAndDislikes.text = GetLocale("clip_result_reaction", clip.Likes.GetDisplay(), clip.Dislikes.GetDisplay());
            fansIncome.text = $"+{clip.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{clip.MoneyIncome.GetMoney()}";
        }
        
        /// <summary>
        /// Сохраняет результаты клипа
        /// </summary>
        private void SaveResult(ClipInfo clip)
        {
            clip.Timestamp = TimeManager.Instance.Now;
            PlayerManager.Instance.GiveReward(clip.FansIncome, clip.MoneyIncome, rewardExp);
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