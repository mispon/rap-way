using Core.Localization;
using Extensions;
using Game.Player.Team;
using Game.Player.Team.Desc;
using Game.Time;
using UI.Windows.GameScreen;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Team
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
            teammateName.text = LocalizationManager.Instance.Get(nameKey).ToUpper();
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