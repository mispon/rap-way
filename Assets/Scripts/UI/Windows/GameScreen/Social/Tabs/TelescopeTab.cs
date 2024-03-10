using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class TelescopeTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button rappingButton;
        [SerializeField] private Button talkingButton;
        [SerializeField] private Button exploreButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            rappingButton.onClick.AddListener(() => ChangeMode(0));
            talkingButton.onClick.AddListener(() => ChangeMode(1));
            exploreButton.onClick.AddListener(() => ChangeMode(2));
        }

        private void ChangeMode(int index)
        {
            _modeIndex = index;
            SetButtonSelected(index);
        }

        private void SetButtonSelected(int index)
        {
            rappingButton.interactable = index != 0;
            talkingButton.interactable = index != 1;
            exploreButton.interactable = index != 2;
        }

        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Telescope,
                MainText = input.text,
                ModeIndex = _modeIndex
            };
        }
        
        protected override void OnOpen()
        {
            base.OnOpen();
            ChangeMode(0);
            hashtag.text = $"#{PlayerAPI.Data.Info.NickName}";
            input.text = string.Empty;
        }
    }
}