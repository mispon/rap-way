using Core.Localization;
using Enums;
using Extensions;
using Game;
using Game.Player.Team;
using MessageBroker;
using MessageBroker.Messages.Player;
using Models.Production;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class TrendsTab : BaseSocialsTab
    {
        [Header("Controls")]
        [SerializeField] private GameObject trends;
        [SerializeField] private Image themeIcon;
        [SerializeField] private Text themeTrend;
        [SerializeField] private Text styleTrend;
        [SerializeField] private GameObject noInfo;
        
        [Header("Data"), SerializeField] private ImagesBank imagesBank;

        protected override SocialInfo GetInfo()
        {
            int cooldown = GameManager.Instance.Settings.Team.PrManagerCooldown;
            MsgBroker.Instance.Publish(new TeammateCooldownMessage
            {
                Type = TeammateType.PrMan,
                Cooldown = cooldown
            });
            
            return new SocialInfo {Type = SocialType.Trends};
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            var lastTrends = PlayerAPI.Data.LastKnownTrends;
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

        protected override bool CheckStartConditions()
        {
            var prManager = PlayerAPI.Data.Team.PrMan;
            return TeamManager.IsAvailable(TeammateType.PrMan) && prManager.Cooldown == 0;
        }
    }
}