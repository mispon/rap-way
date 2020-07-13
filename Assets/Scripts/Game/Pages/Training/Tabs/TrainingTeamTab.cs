using System;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Вкладка тренировки команды
    /// </summary>
    public class TrainingTeamTab : TrainingTab
    {
        [Header("Тиммейты"), Tooltip("Порядок элементов аналогичен PlayerTeam.TeammatesArray")]
        [SerializeField] private GameObject[] teammates;

        [Header("Контролы")]
        [SerializeField] private Text bitmakerLevel;
        [SerializeField] private Button upBitmakerButton;
        [Space, SerializeField] private Text textwritterLevel;
        [SerializeField] private Button upTextwritterButton;
        [Space, SerializeField] private Text managerLevel;
        [SerializeField] private Button upManagerButton;
        [Space, SerializeField] private Text prmanLevel;
        [SerializeField] private Button upPrmanButton;
        
        private readonly Func<string>[] _finishCallbacks =
        {
            () => FinishCallback(() => PlayerManager.Data.Team.BitMaker.Skill += 1, "teammate_bitmaker"),
            () => FinishCallback(() => PlayerManager.Data.Team.TextWriter.Skill += 1, "teammate_textwritter"),
            () => FinishCallback(() => PlayerManager.Data.Team.Manager.Skill += 1, "teammate_manager"),
            () => FinishCallback(() => PlayerManager.Data.Team.PrMan.Skill += 1, "teammate_prman")
        };
        
        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            upBitmakerButton.onClick.AddListener(() => OnTeammateTrain(_finishCallbacks[0]));
            upTextwritterButton.onClick.AddListener(() => OnTeammateTrain(_finishCallbacks[1]));
            upManagerButton.onClick.AddListener(() => OnTeammateTrain(_finishCallbacks[2]));
            upPrmanButton.onClick.AddListener(() => OnTeammateTrain(_finishCallbacks[3]));
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            var team = PlayerManager.Data.Team;
            textwritterLevel.text = $"{Locale("level")}: {team.TextWriter.Skill}";
            bitmakerLevel.text = $"{Locale("level")}: {team.BitMaker.Skill}";
            managerLevel.text = $"{Locale("level")}: {team.Manager.Skill}";
            prmanLevel.text = $"{Locale("level")}: {team.PrMan.Skill}";

            for (int i = 0; i < teammates.Length; i++)
            {
                var teammate = team.TeammatesArray[i];
                var isAvailableToTrain = !teammate.IsEmpty && teammate.Skill < Teammate.MAX_SKILL;
                teammates[i].SetActive(isAvailableToTrain);
            }
        }

        private void OnTeammateTrain(Func<string> onFinish)
        {
            onStartTraining.Invoke(trainingDuration, onFinish);
        }

        /// <summary>
        /// Обработчик завершения тренировки тиммейта 
        /// </summary>
        private static string FinishCallback(Action action, string teammateKey)
        {
            action.Invoke();
            return $"{Locale("training_teammateUpgrade")}: {Locale(teammateKey)}";
        }
    }
}