using Core;
using Models.Player;
using UnityEngine;

namespace Game.Pages.Team
{
    /// <summary>
    /// Страница зарплат команды
    /// </summary>
    public class TeammateSalaryPage: Page
    {
        [SerializeField] private TeamManager teamManager;
        [SerializeField] private SalaryElementsContainer[] teammateElementsTemplates;

        private Teammate[] _awaitForPaymentTeammates;

        /// <summary>
        /// Открытие страницы "продления трудового договора"
        /// </summary>
        public void Show(Teammate[] awaitForPaymentTeammates)
        {
            _awaitForPaymentTeammates = awaitForPaymentTeammates;
            Open();
        }

        protected override void BeforePageOpen()
        {
            TimeManager.Instance.SetFreezed(true);
            
            foreach (var teammate in _awaitForPaymentTeammates)
                teammate.HasPayment = false;
            
            for (var i = 0; i < _awaitForPaymentTeammates.Length; i++)
            {
                var teammate = _awaitForPaymentTeammates[i];
                teammateElementsTemplates[i].Setup(teammate, teamManager.GetSalary(teammate));
            }
        }

        protected override void BeforePageClose()
        {
            _awaitForPaymentTeammates = null;
            
            foreach (var page in teammateElementsTemplates)
                page.Reset();
            
            TimeManager.Instance.SetFreezed(false);
        }
    }
}