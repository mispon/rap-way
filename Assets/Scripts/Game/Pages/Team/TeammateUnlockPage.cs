using Core;
using Localization;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Team
{
    /// <summary>
    /// Страница открытия нового тиммейта
    /// </summary>
    public class TeammateUnlockPage: Page
    {
        [Space, SerializeField] private Text teammateText;
        private Teammate _unlockedTeammate;

        /// <summary>
        /// Открытие страницы отображения нового члена команды
        /// </summary>
        public void Show(Teammate unlockedTeammate)
        {
            _unlockedTeammate = unlockedTeammate;
            Open();
        }
        
        protected override void AfterPageOpen()
        {
            TimeManager.Instance.SetFreezed(true);
            
            teammateText.text = 
                LocalizationManager.Instance.Get(_unlockedTeammate.Type.GetDescription());   
        }

        protected override void BeforePageClose()
        {
            _unlockedTeammate.Skill = 1;
            _unlockedTeammate.HasPayment = true;
            _unlockedTeammate = null;
            
            TimeManager.Instance.SetFreezed(false);
        }
    }
}