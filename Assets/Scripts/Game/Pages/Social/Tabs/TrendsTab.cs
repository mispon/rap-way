using Data;
using Enums;
using Localization;
using Models.Info;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Social.Tabs
{
    /// <summary>
    /// Вкладка трендов
    /// </summary>
    public class TrendsTab : BaseSocialsTab
    {
        [Header("Поля")]
        [SerializeField] private GameObject trends;
        [SerializeField] private Image themeIcon;
        [SerializeField] private Text themeTrend;
        [SerializeField] private Text styleTrend;
        [Space]
        [SerializeField] private GameObject noInfo;

        [Header("Иконки")]
        [SerializeField] private ImagesBank imagesBank;

        /// <summary>
        /// Возвращает информацию соц. действия
        /// </summary>
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Trends
            };
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            var lastTrends = PlayerManager.Data.LastKnownTrends;
            if (lastTrends != null)
            {
                themeIcon.sprite = imagesBank.ThemesActive[(int) lastTrends.Theme];
                themeTrend.text = LocalizationManager.Instance.Get(lastTrends.Theme.GetDescription()).ToUpper();
                styleTrend.text = LocalizationManager.Instance.Get(lastTrends.Style.GetDescription()).ToUpper();
            }

            ToggleInfoBlocks(lastTrends != null);
        }

        private void ToggleInfoBlocks(bool hasTrendInfo)
        {
            trends.SetActive(hasTrendInfo);
            noInfo.SetActive(!hasTrendInfo);
        }
    }
}