using Core.Localization;
using Enums;
using Extensions;
using Game;
using Game.Player;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.Tabs
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
            PlayerManager.SetTeammateCooldown(TeammateType.PrMan, GameManager.Instance.Settings.PrManagerCooldown);
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

        /// <summary>
        /// Проверяет условия запуска соц. действия
        /// </summary>
        protected override bool CheckStartConditions()
        {
            var prManager = PlayerManager.Data.Team.PrMan;
            return TeamManager.IsAvailable(TeammateType.PrMan) && prManager.Cooldown == 0;
        }
    }
}