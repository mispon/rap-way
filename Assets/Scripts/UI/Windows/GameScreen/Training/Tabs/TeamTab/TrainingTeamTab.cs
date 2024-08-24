using Core;
using Game.Player.Team.Desc;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.TeamTab
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

        public override void Init()
        {
            foreach (var teammateCard in teammatesCards)
            {
                teammateCard.onUp += AddExp;
                teammateCard.onPay += GivePayment;
            }
        }

        protected override void OnOpen()
        {
            var teammates = PlayerAPI.Data.Team.TeammatesArray;

            for (int i = 0; i < teammatesCards.Length; i++)
            {
                var member = teammates[i];
                bool expEnough = PlayerAPI.Data.Exp >= trainingCost;

                teammatesCards[i].Setup(member, expEnough, expToLevelUp[member.Skill.Value]);
            }
        }

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

        private void GivePayment(Teammate teammate, int salary)
        {
            MsgBroker.Instance.Publish(new ChangeMoneyMessage { Amount = -salary });
            teammate.HasPayment = true;

            SoundManager.Instance.PlaySound(UIActionType.Pay);
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