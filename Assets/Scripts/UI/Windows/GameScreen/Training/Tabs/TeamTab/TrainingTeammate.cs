using System;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game.Player.State.Desc;
using Game.Player.Team;
using Game.Player.Team.Desc;
using UI.Controls.Progress;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.TeamTab
{
    /// <summary>
    /// Карточка тренируемого тиммейта
    /// </summary>
    public class TrainingTeammate : MonoBehaviour
    {
        [SerializeField] private Image avatar;
        [SerializeField] private Text level;
        [SerializeField] private Button upButton;
        [SerializeField] private Button payButton;
        [SerializeField] private ProgressBar expBar;
        [SerializeField] private GameObject salaryLabel;
        [SerializeField] private Text salaryText;
        [SerializeField] private GameObject inactiveLabel;
        [Space]
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;

        public Action<Teammate> onUp = teammate => {};
        public Action<Teammate, int> onPay = (teammate, salary) => {};

        private Teammate _teammate;

        private void Start()
        {
            upButton.onClick.AddListener(OnUp);
            payButton.onClick.AddListener(OnPay);
        }

        public void Setup(Teammate teammate, bool expEnough, int expToUp)
        {
            _teammate = teammate;
            
            if (teammate.IsEmpty)
            {
                SetLocked();
                return;
            }

            int salaryAmount = TeamManager.Instance.GetSalary(teammate);
            salaryText.text = salaryAmount.GetMoney();
            salaryLabel.SetActive(!teammate.HasPayment);

            avatar.sprite = teammate.HasPayment ? activeSprite : inactiveSprite;
            inactiveLabel.SetActive(false);
            level.text = teammate.Skill.Value.ToString();
            
            bool noLimit = teammate.Skill.Value < PlayerData.MAX_SKILL;
            
            int exp = noLimit ? teammate.Skill.Exp : expToUp;
            expBar.SetValue(exp, expToUp);

            upButton.interactable = noLimit && expEnough;
            payButton.interactable = !teammate.HasPayment && salaryAmount <= PlayerAPI.Data.Money;
        }

        /// <summary>
        /// Устанавливает в закрытое состояние
        /// </summary>
        private void SetLocked()
        {
            avatar.sprite = inactiveSprite;
            inactiveLabel.SetActive(true);
            level.text = "0";
            
            expBar.SetValue(0, 1);
                
            upButton.interactable = false;
            payButton.interactable = false;
        }

        private void OnUp()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrainingTeammateUpgraded);
            
            onUp.Invoke(_teammate);
        }

        private void OnPay()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrainingTeammateSalaryPayed);
            
            var salary = TeamManager.Instance.GetSalary(_teammate);
            onPay.Invoke(_teammate, salary);
        }
    }
}