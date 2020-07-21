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
        [Space, SerializeField] private Text teammateText;

        /// <summary>
        /// Открытие страницы отображения нового члена команды
        /// </summary>
        public void Show(Teammate unlockedTeammate)
        {
            var desc = unlockedTeammate.Type.GetDescription();
            teammateText.text = LocalizationManager.Instance.Get(desc); 
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