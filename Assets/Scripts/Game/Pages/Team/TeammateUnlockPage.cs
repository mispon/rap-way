using Enums;
using Localization;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Team
{
    public class TeammateUnlockPage: Page
    {
        [Space, SerializeField] private Text teammateText;
        private Teammate _unlockedTeammate;

        public void Show(Teammate unlockedTeammate)
        {
            _unlockedTeammate = unlockedTeammate;
            Open();
        }
        protected override void AfterPageOpen()
        {
            teammateText.text = 
                LocalizationManager.Instance.Get(_unlockedTeammate.type.GetDescription());   
        }

        protected override void BeforePageClose()
        {
            _unlockedTeammate.Skill = 1;
            _unlockedTeammate.hasPayment = true;

            _unlockedTeammate = null;
        }
    }
}