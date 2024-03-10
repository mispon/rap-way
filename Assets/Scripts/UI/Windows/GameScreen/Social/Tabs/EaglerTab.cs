using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Social.Tabs
{
    public class EaglerTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;

        protected override SocialInfo GetInfo()
        {
            return new SocialInfo
            {
                Type = SocialType.Eagler,
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