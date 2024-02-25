using Enums;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Social.Tabs
{
    /// <summary>
    /// Вкладка псевдо-твича
    /// </summary>
    public class SwitchTab : BaseSocialsTab
    {
        [SerializeField] private InputField input;

        /// <summary>
        /// Возвращает информацию соц. действия 
        /// </summary>
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