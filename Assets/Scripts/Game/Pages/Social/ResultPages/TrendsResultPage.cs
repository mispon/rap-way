using Core;
using Data;
using Firebase.Analytics;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.ResultPages
{
    /// <summary>
    /// Страница результата исследования трендов
    /// </summary>
    public class TrendsResultPage : SocialResultPage
    {
        [Header("Контролы")]
        [SerializeField] private Image themeIcon;
        [SerializeField] private Text themeName;
        [SerializeField] private Text styleName;

        [Header("Картинки")]
        [SerializeField] private ImagesBank imagesBank;

        /// <summary>
        /// Отображает результаты соц. действия
        /// </summary>
        protected override void DisplayResult(SocialInfo socialInfo)
        {
            var trends = GameManager.Instance.GameStats.Trends;

            themeIcon.sprite = imagesBank.ThemesActive[(int) trends.Theme];
            themeName.text = GetLocale(trends.Theme.GetDescription()).ToUpper();
            styleName.text = GetLocale(trends.Style.GetDescription()).ToUpper();

            PlayerManager.UpdateTrends(trends.Style, trends.Theme);
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrandsAnalyzed);
        }
    }
}