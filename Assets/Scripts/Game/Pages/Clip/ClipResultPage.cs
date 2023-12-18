using System;
using Core;
using Game.Analyzers;
using Game.Pages.Eagler;
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
        [SerializeField] private Text views;
        [SerializeField] private Text likes;
        [SerializeField] private Text dislikes;
        [SerializeField] private Text clipNameLabel;
        [SerializeField] private Text playerNameLabel;
        [SerializeField] private Text qualityLabel;
        [SerializeField] private Text fansIncome;
        [SerializeField] private Text moneyIncome;
        [SerializeField] private Text expIncome;
        [SerializeField] private GameObject hitBadge;
        
        [Header("Твитты фанатов")]
        [SerializeField] private EagleCard[] eagleCards;

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
            string fansPrefix = clip.FansIncome > 0 ? "+" : string.Empty;
            fansIncome.text = $"{fansPrefix}{clip.FansIncome.GetDisplay()}";
            moneyIncome.text = $"+{clip.MoneyIncome.GetMoney()}";
            expIncome.text = $"+{settings.ClipRewardExp}";

            clipNameLabel.text = ProductionManager.GetTrackName(clip.TrackId);
            playerNameLabel.text = PlayerManager.Data.Info.NickName;
            qualityLabel.text = $"{Convert.ToInt32(clip.Quality * 100)}%";
            
            views.text = clip.Views.GetDisplay();
            likes.text = clip.Likes.GetDisplay();
            dislikes.text = clip.Dislikes.GetDisplay();
            
            hitBadge.SetActive(clip.IsHit);
            
            DisplayEagles(clip.Quality);
        }
        
        private void DisplayEagles(float quality)
        {
            var eagles = EaglerManager.Instance.GenerateEagles(quality);
            for (var i = 0; i < eagles.Count; i++)
            {
                eagleCards[i].Initialize(i, eagles[i]);
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
            
            GameManager.Instance.SaveApplicationData();
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