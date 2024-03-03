using Core.Context;
using Game.Player;
using Game.Player.State.Desc;
using Game.Rappers.Desc;
using Models.Production;
using Sirenix.OdinInspector;
using UI.Windows.GameScreen.Track;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Feat
{
    public class FeatSettingsPage : TrackSettingsPage
    {
        [BoxGroup("Settings"), SerializeField] private Text rapperName;
        [BoxGroup("Settings"), SerializeField] private Image rapperAvatar;
        [BoxGroup("Settings"), SerializeField] private Sprite customRapperAvatar;

        private RapperInfo _rapper;

        public override void Show(object ctx = null)
        {
            _rapper = ctx.Value<RapperInfo>();
            base.Show(ctx);
        }

        private void DisplaySkills(PlayerData data)
        {
            int playerBitSkill = data.Stats.Bitmaking.Value;
            int rapperBitSkill = _rapper.Bitmaking;
            bitSkill.text = $"{playerBitSkill + rapperBitSkill}";
            
            int playerTextSkill = data.Stats.Vocobulary.Value;
            int rapperTextSkill = _rapper.Vocobulary;
            textSkill.text = $"{playerTextSkill + rapperTextSkill}";
        }
        
        protected override void BeforeShow()
        {
            _track = new TrackInfo {Feat = _rapper};

            rapperName.text = _rapper.Name;
            rapperAvatar.sprite = _rapper.IsCustom ? customRapperAvatar : _rapper.Avatar;
            
            SetupCarousel(PlayerManager.Data);
            DisplaySkills(PlayerManager.Data);
        }

        protected override void AfterHide()
        {
            base.AfterHide();
            _rapper = null;
        }
    }
}