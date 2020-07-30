using Core;
using Localization;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Team
{
    /// <summary>
    /// Страница открытия нового тиммейта
    /// </summary>
    public class TeammateUnlockPage: Page
    {
        [SerializeField] private Text teammateName;
        [SerializeField] private Image avatar;

        /// <summary>
        /// Открытие страницы отображения нового члена команды
        /// </summary>
        public void Show(Teammate unlockedTeammate, Sprite teammateAvatar)
        {
            var nameKey = unlockedTeammate.Type.GetDescription();
            teammateName.text = LocalizationManager.Instance.Get(nameKey);
            avatar.sprite = teammateAvatar;
            
            Open();
        }
        
        protected override void AfterPageOpen()
        {
            TimeManager.Instance.SetFreezed(true);
        }

        protected override void BeforePageClose()
        {
            TimeManager.Instance.SetFreezed(false);
        }
    }
}