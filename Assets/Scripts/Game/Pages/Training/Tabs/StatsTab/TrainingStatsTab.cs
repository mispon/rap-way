using System;
using Data;
using Models.Game;
using UnityEngine;

namespace Game.Pages.Training.Tabs.StatsTab
{
    /// <summary>
    /// Вкладка тренировки навыков
    /// </summary>
    public class TrainingStatsTab : TrainingTab
    {
        [Header("Карточки навыков")]
        [SerializeField] private TrainingStatsCard[] statsCards;

        [Header("Данные о навыках")]
        [SerializeField] private TrainingInfoData data;
        
        [Header("Настройки тренировок")]
        [Tooltip("Количество опыта на одно нажатие")]
        [SerializeField] private int trainingCost;
        [Tooltip("Требуемый опыт для увеличения уровня")]
        [SerializeField] private int[] expToLevelUp;

        private int _statIndex;
        private Func<int>[] _trainingActions => new Func<int>[]
        {
            UpgradeVocobulary,
            UpgradeBitmaking,
            UpgradeFlow,
            UpgradeCharisma,
            UpgradeManagement,
            UpgradeMarketing
        };
        
        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            for (int i = 0; i < statsCards.Length; i++)
            {
                var statButton = statsCards[i];
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
            OnStatsSelected(_statIndex);
        }

        /// <summary>
        /// Обработчик выбора навыка 
        /// </summary>
        private void OnStatsSelected(int index)
        {
            _statIndex = index;
            
            for (int i = 0; i < statsCards.Length; i++)
            {
                if (i == index)
                {
                    var info = data.StatsInfo[index];
                    var stat = PlayerManager.Data.Stats.Values[index];
                    bool expEnough = PlayerManager.Data.Exp >= trainingCost;

                    statsCards[i].Show(info, expEnough, expToLevelUp[stat.Value]);
                }
                else
                    statsCards[i].Hide();
            }
        }
        
        /// <summary>
        /// Обработчик запуска улучшения навыка
        /// </summary>
        private void OnUpgradeStats(int index)
        {
            int cost = _trainingActions[index].Invoke();
            onStartTraining.Invoke(() => cost);
        }

        /// <summary>
        /// Группа методов прокачки каждого из статов персонажа 
        /// </summary>
        private int UpgradeVocobulary()  =>  AddExp(ref PlayerManager.Data.Stats.Vocobulary);
        private int UpgradeBitmaking()   =>  AddExp(ref PlayerManager.Data.Stats.Bitmaking);
        private int UpgradeFlow()        =>  AddExp(ref PlayerManager.Data.Stats.Flow);
        private int UpgradeCharisma()    =>  AddExp(ref PlayerManager.Data.Stats.Charisma);
        private int UpgradeManagement()  =>  AddExp(ref PlayerManager.Data.Stats.Management);
        private int UpgradeMarketing()   =>  AddExp(ref PlayerManager.Data.Stats.Marketing);

        /// <summary>
        /// Добавляет опыт к переданному стату 
        /// </summary>
        private int AddExp(ref ExpValue stat)
        {
            int expToUp = expToLevelUp[stat.Value];

            stat.Exp += trainingCost;

            if (stat.Exp >= expToUp)
            {
                stat.Value += 1;
                stat.Exp -= expToUp;
            }
            
            return trainingCost;
        }

        private void OnDestroy()
        {
            foreach (var statsButton in statsCards)
            {
                statsButton.onClick -= OnStatsSelected;
                statsButton.onLevelUpClick -= OnUpgradeStats;
            }
        }
    }
}