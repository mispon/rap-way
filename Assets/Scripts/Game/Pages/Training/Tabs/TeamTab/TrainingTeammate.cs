using System;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs.TeamTab
{
    /// <summary>
    /// Карточка тренируемого тиммейта
    /// </summary>
    public class TrainingTeammate : MonoBehaviour
    {
        [SerializeField] private Text level;
        [SerializeField] private Button upButton;
        [SerializeField] private Button payButton;
        [SerializeField] private Slider exp;
        [SerializeField] private GameObject innactiveLabel;

        public Action<Teammate> onUp = teammate => {};
        public Action<Teammate, int> onPay = (teammate, salary) => {};

        private Teammate _teammate;

        private void Start()
        {
            upButton.onClick.AddListener(OnUp);
            payButton.onClick.AddListener(OnPay);
        }

        /// <summary>
        /// Инициализирует поля
        /// </summary>
        public void Setup(Teammate teammate, bool expEnough, int expToUp)
        {
            _teammate = teammate;
            
            if (teammate.IsEmpty)
            {
                SetLocked();
                return;
            }
            
            innactiveLabel.SetActive(false);
            level.text = teammate.Skill.Value.ToString();
            
            bool noLimit = teammate.Skill.Value < PlayerData.MAX_SKILL;
            
            exp.maxValue = expToUp;
            exp.value = noLimit ? teammate.Skill.Exp : expToUp;

            upButton.interactable = noLimit && expEnough;
            payButton.interactable = !teammate.HasPayment;
        }

        /// <summary>
        /// Устанавливает в закрытое состояние
        /// </summary>
        private void SetLocked()
        {
            innactiveLabel.SetActive(true);
            level.text = "0";
            
            exp.value = 0;
            exp.maxValue = 1;
                
            upButton.interactable = false;
            payButton.interactable = false;
        }

        /// <summary>
        /// Обработчик повышения уровня
        /// </summary>
        private void OnUp()
        {
            onUp.Invoke(_teammate);
        }

        /// <summary>
        /// Обработчик выдачи зарплаты
        /// </summary>
        private void OnPay()
        {
            var salaty = TeamManager.Instance.GetSalary(_teammate);
            onPay.Invoke(_teammate, salaty);
        }
    }
}