using Extensions;
// using Firebase.Analytics;
using Game;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.ResultPages
{
    public class TrendsResultPage : SocialResultPage
    {
        [Header("controls")]
        [SerializeField] private Image themeIcon;
        [SerializeField] private Text themeName;
        [SerializeField] private Text styleName;

        [Header("Data"), SerializeField] private ImagesBank imagesBank;

        protected override void DisplayResult(SocialInfo socialInfo)
        {
            var trends = GameManager.Instance.GameStats.Trends;

            themeIcon.sprite = imagesBank.ThemesActive[(int) trends.Theme];
            themeName.text = GetLocale(trends.Theme.GetDescription()).ToUpper();
            styleName.text = GetLocale(trends.Style.GetDescription()).ToUpper();

            PlayerAPI.UpdateKnownTrends(trends.Style, trends.Theme);
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrandsAnalyzed);
        }
    }
}