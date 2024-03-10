using System;
using Core;
using Models.Game;
using ScriptableObjects;
using UnityEngine;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.StatsTab
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
            UpgradeVocabulary,
            UpgradeBitmaking,
            UpgradeFlow,
            UpgradeCharisma,
            UpgradeManagement,
            UpgradeMarketing
        };
        
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

        protected override void OnOpen()
        {
            OnStatsSelected(_statIndex);
        }

        private void OnStatsSelected(int index)
        {
            _statIndex = index;
            
            for (int i = 0; i < statsCards.Length; i++)
            {
                if (i == index)
                {
                    var info = data.StatsInfo[index];
                    var stat = PlayerAPI.Data.Stats.Values[index];
                    bool expEnough = PlayerAPI.Data.Exp >= trainingCost;

                    statsCards[i].Show(info, expEnough, expToLevelUp[stat.Value]);
                }
                else
                    statsCards[i].Hide();
            }
        }
        
        private void OnUpgradeStats(int index)
        {
            SoundManager.Instance.PlaySound(UIActionType.Train);
            int cost = _trainingActions[index].Invoke();
            onStartTraining.Invoke(() => cost);
        }

        /// <summary>
        /// Группа методов прокачки каждого из статов персонажа 
        /// </summary>
        private int UpgradeVocabulary() => AddExp(ref PlayerAPI.Data.Stats.Vocobulary);
        private int UpgradeBitmaking()  => AddExp(ref PlayerAPI.Data.Stats.Bitmaking);
        private int UpgradeFlow()       => AddExp(ref PlayerAPI.Data.Stats.Flow);
        private int UpgradeCharisma()   => AddExp(ref PlayerAPI.Data.Stats.Charisma);
        private int UpgradeManagement() => AddExp(ref PlayerAPI.Data.Stats.Management);
        private int UpgradeMarketing()  => AddExp(ref PlayerAPI.Data.Stats.Marketing);
        
        private int AddExp(ref ExpValue stat)
        {
            int expToUp = expToLevelUp[stat.Value];

            stat.Exp += trainingCost;

            if (stat.Exp >= expToUp)
            {
                stat.Value += 1;
                stat.Exp -= expToUp;
                SoundManager.Instance.PlaySound(UIActionType.LevelUp);
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