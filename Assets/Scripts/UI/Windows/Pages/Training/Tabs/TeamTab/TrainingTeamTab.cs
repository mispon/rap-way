using Core;
using Game.Player;
using MessageBroker;
using MessageBroker.Messages.State;
using Models.Player;
using ScriptableObjects;
using UnityEngine;

namespace UI.Windows.Pages.Training.Tabs.TeamTab
{
    /// <summary>
    /// Вкладка тренировки команды
    /// </summary>
    public class TrainingTeamTab : TrainingTab
    {
        [Header("Тиммейты")]
        [Tooltip("Порядок элементов аналогичен PlayerTeam.TeammatesArray")]
        [SerializeField] private TrainingTeammate[] teammatesCards;

        [Header("Настройки тренировок")]
        [Tooltip("Количество опыта на одно нажатие")]
        [SerializeField] private int trainingCost;
        [Tooltip("Требуемый опыт для увеличения уровня")]
        [SerializeField] private int[] expToLevelUp;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            foreach (var teammateCard in teammatesCards)
            {
                teammateCard.onUp += AddExp;
                teammateCard.onPay += GivePayment;
            }
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            var teammates = PlayerManager.Data.Team.TeammatesArray;

            for (int i = 0; i < teammatesCards.Length; i++)
            {
                var member = teammates[i];
                bool expEnough = PlayerManager.Data.Exp >= trainingCost;
                
                teammatesCards[i].Setup(member, expEnough, expToLevelUp[member.Skill.Value]);
            }
        }

        /// <summary>
        /// Добавляет очки опыта 
        /// </summary>
        private void AddExp(Teammate teammate)
        {
            bool isLevelUp = false;
            int expToUp = expToLevelUp[teammate.Skill.Value];
            
            teammate.Skill.Exp += trainingCost;

            if (teammate.Skill.Exp >= expToUp)
            {
                teammate.Skill.Value += 1;
                teammate.Skill.Exp -= expToUp;
                isLevelUp = true;
            }

            SoundManager.Instance.PlaySound(isLevelUp ? UIActionType.LevelUp : UIActionType.Train);
            onStartTraining.Invoke(() => trainingCost);
        }
        
        /// <summary>
        /// Выдает зарплату указанному тиммейту 
        /// </summary>
        private void GivePayment(Teammate teammate, int salary)
        {
            MainMessageBroker.Instance.Publish(new ChangeMoneyEvent {Amount = -salary});
            teammate.HasPayment = true;
  
            onStartTraining.Invoke(() => 0);
        }

        private void OnDestroy()
        {
            foreach (var teammateCard in teammatesCards)
            {
                teammateCard.onUp -= AddExp;
                teammateCard.onPay -= GivePayment;
            }
        }
    }
}