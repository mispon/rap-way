using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class IeyegramTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;
        [SerializeField] private Text hashtag;
        [Space]
        [SerializeField] private Button lifeButton;
        [SerializeField] private Button foodButton;
        [SerializeField] private Button musicButton;

        private int _modeIndex;
        
        protected override void TabStart()
        {
            lifeButton.onClick.AddListener(() => ChangeMode(0));
            foodButton.onClick.AddListener(() => ChangeMode(1));
            musicButton.onClick.AddListener(() => ChangeMode(2));
        }

        private void ChangeMode(int index)
        {
            _modeIndex = index;
            SetButtonSelected(index);
        }

        private void SetButtonSelected(int index)
        {
            lifeButton.interactable = index != 0;
            foodButton.interactable = index != 1;
            musicButton.interactable = index != 2;
        }

        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Ieyegram,
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