using Data;
using Game.Pages.Track;
using Game.UI.GameScreen;
using Models.Info.Production;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Feat
{
    /// <summary>
    /// Страница настройки фита
    /// </summary>
    public class FeatSettingsPage : TrackSettingsPage
    {
        [Header("Настройки фита")]
        [SerializeField] private Text rapperName;
        [SerializeField] private Image rapperAvatar;

        private RapperInfo _rapper;
        
        /// <summary>
        /// Открывает страницу настроек
        /// </summary>
        public void Show(RapperInfo rapper)
        {
            _rapper = rapper;
            Open();
        }

        /// <summary>
        /// Показывает текущий суммарный скилл команды 
        /// </summary>
        private void DisplaySkills(PlayerData data)
        {
            int playerBitSkill = data.Stats.Bitmaking.Value;
            int rapperBitSkill = _rapper.Bitmaking;
            bitSkill.text = $"{playerBitSkill + rapperBitSkill}";
            
            int playerTextSkill = data.Stats.Vocobulary.Value;
            int rapperTextSkill = _rapper.Vocobulary;
            textSkill.text = $"{playerTextSkill + rapperTextSkill}";
        }
        
        protected override void BeforePageOpen()
        {
            _track = new TrackInfo {Feat = _rapper};

            rapperName.text = _rapper.Name;
            rapperAvatar.sprite = _rapper.Avatar;
            
            SetupCarousel(PlayerManager.Data);
            DisplaySkills(PlayerManager.Data);
            
            GameScreenController.Instance.HideProductionGroup();
        }

        protected override void AfterPageClose()
        {
            base.AfterPageClose();
            _rapper = null;
        }
    }
}