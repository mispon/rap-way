using System;
using Data;
using UnityEngine;

namespace Game.Pages.Training.Tabs.StatsTab
{
    /// <summary>
    /// Вкладка тренировки навыков
    /// </summary>
    public class TrainingStatsTab : TrainingTab
    {
        [Header("Карточки навыков")]
        [SerializeField] private StatsButton[] statsButtons;

        [Header("Данные о навыках")]
        [SerializeField] private TrainingInfoData data;
        
        private readonly Func<string>[] _finishCallbacks =
        {
            () => FinishCallback(() => PlayerManager.Data.Stats.Vocobulary += 1, "vocobulary"),
            () => FinishCallback(() => PlayerManager.Data.Stats.Bitmaking += 1, "bitmaking"),
            () => FinishCallback(() => PlayerManager.Data.Stats.Flow += 1, "flow"),
            () => FinishCallback(() => PlayerManager.Data.Stats.Charisma += 1, "charisma"),
            () => FinishCallback(() => PlayerManager.Data.Stats.Management += 1, "management"),
            () => FinishCallback(() => PlayerManager.Data.Stats.Marketing += 1, "marketing")
        };

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            for (int i = 0; i < statsButtons.Length; i++)
            {
                var statButton = statsButtons[i];
                statButton.SetIndex(i);
                statButton.onClick += OnStatsSelected;
                statButton.onLevelUpClick += OnUpgradeStats;
            }
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            OnStatsSelected(0);
        }

        /// <summary>
        /// Обработчик выбора навыка 
        /// </summary>
        private void OnStatsSelected(int index)
        {
            for (int i = 0; i < statsButtons.Length; i++)
            {
                if (i == index)
                {
                    var info = data.StatsInfo[index];
                    statsButtons[i].Show(info);
                }
                else
                    statsButtons[i].Hide();
            }
        }
        
        /// <summary>
        /// Обработчик запуска улучшения навыка
        /// </summary>
        private void OnUpgradeStats(int index)
        {
            var onFinish = _finishCallbacks[index];
            onStartTraining.Invoke(trainingDuration, onFinish);
        }

        /// <summary>
        /// Обработчик завершения тренировки навыка 
        /// </summary>
        private static string FinishCallback(Action action, string statKey)
        {
            action.Invoke();
            return $"{Locale("training_statUpgrade")}: {Locale(statKey)}";
        }

        private void OnDestroy()
        {
            foreach (var statsButton in statsButtons)
            {
                statsButton.onClick -= OnStatsSelected;
                statsButton.onLevelUpClick -= OnUpgradeStats;
            }
        }
    }
}