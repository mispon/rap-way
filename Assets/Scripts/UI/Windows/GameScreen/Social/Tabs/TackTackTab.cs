using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class TackTackTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button challengeButton;
        [SerializeField] private Button danceButton;
        [SerializeField] private Button memeButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            challengeButton.onClick.AddListener(() => ChangeMode(0));
            danceButton.onClick.AddListener(() => ChangeMode(1));
            memeButton.onClick.AddListener(() => ChangeMode(2));
        }

        private void ChangeMode(int index)
        {
            _modeIndex = index;
            SetButtonSelected(index);
        }

        private void SetButtonSelected(int index)
        {
            challengeButton.interactable = index != 0;
            danceButton.interactable = index != 1;
            memeButton.interactable = index != 2;
        }
        
        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.TackTack,
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