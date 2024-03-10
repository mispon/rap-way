using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class SwitchTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;

        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Switch,
                MainText = input.text
            };
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            input.text = string.Empty;
        }
    }
}