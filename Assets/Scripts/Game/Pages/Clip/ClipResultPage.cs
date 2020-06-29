using Game.Analyzers;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Анализатор клипа")]
        [SerializeField] private ClipAnalyzer clipAnalyzer;
        
        /// <summary>
        /// Показывает результат работы над клипом
        /// </summary>
        public void Show(ClipInfo clip)
        {
            clipAnalyzer.Analyze(clip);
            DisplayResult(clip);
            SaveResult(clip);
            Open();
        }

        /// <summary>
        /// Выводит результат работы 
        /// </summary>
        private void DisplayResult(ClipInfo clip)
        {
            header.text = $"Завершена работа над клипом трека \"{PlayerManager.GetTrackName(clip.TrackId)}\"";
            viewsAmount.text = $"ПРОСМОТРЫ: {clip.Views}";
            likesAndDislikes.text = $"{clip.Likes} лайков / {clip.Dislikes} дизлайков";
            fansIncome.text = $"ФАНАТЫ: +{clip.FansIncome}";
            moneyIncome.text = $"ДЕНЬГИ: +{clip.MoneyIncome}";
        }
        
        /// <summary>
        /// Сохраняет результаты клипа
        /// </summary>
        private static void SaveResult(ClipInfo clip)
        {
            PlayerManager.Instance.GiveReward(clip.FansIncome, clip.MoneyIncome);
            PlayerManager.Data.History.ClipList.Add(clip);
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