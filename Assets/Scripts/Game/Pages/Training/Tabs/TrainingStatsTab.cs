using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Вкладка тренировки навыков
    /// </summary>
    public class TrainingStatsTab : TrainingTab
    {
        private const int MAX_STAT_LEVEL = 10;

        [Header("Основное")]
        [SerializeField] private Button[] statsButtons;
        
        [Header("Информация о навыке")]
        [SerializeField] private Text header;
        [SerializeField] private Text desc;
        [SerializeField] private Text level;
        [SerializeField] private Button upButton;

        [Header("Данные о навыках")]
        [SerializeField] private TrainingInfoData data;

        private int _statsIndex;
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
                int index = i;
                statsButtons[index].onClick.AddListener(() => OnStatsSelected(index));
            }
            
            upButton.onClick.AddListener(OnUpgradeStats);
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
            _statsIndex = index;
            
            var stat = PlayerManager.Data.Stats.Values[index];
            
            header.text = Locale(data.StatsInfo[index].NameKey);
            desc.text = Locale(data.StatsInfo[index].DescriptionKey);
            level.text = $"{Locale("level")}: {stat}";

            upButton.gameObject.SetActive(stat < MAX_STAT_LEVEL);
        }
        
        /// <summary>
        /// Обработчик запуска улучшения навыка
        /// </summary>
        private void OnUpgradeStats()
        {
            var onFinish = _finishCallbacks[_statsIndex];
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
    }
}