using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.SkillsTab
{    
    /// <summary>
    /// Вкладка тренировки умений
    /// </summary>
    public class TrainingSkillsTab : TrainingTab
    {
        [Header("Цена покупки нового умения")]
        [SerializeField] private int skillCost;

        [Header("Элемнты вкладки")]
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private TrainingSkillCard cardTemplate;

        [Header("Данные")]
        [SerializeField] private TrainingInfoData data;

        private List<TrainingSkillCard> _skillCards;

        public override void Init()
        {
            var skills = (Skills[]) Enum.GetValues(typeof(Skills));
            _skillCards = new List<TrainingSkillCard>(skills.Length);

            for (var i = 0; i < skills.Length; i++)
            {
                var info = data.SkillsInfo.First(e => e.Type == skills[i]);
                var card = Instantiate(cardTemplate, grid.transform);
                
                card.Setup(i, info);
                card.onUnlock += OnLearnSkill;

                _skillCards.Add(card);
            }
        }

        protected override void OnOpen()
        {
            bool expEnough = PlayerAPI.Data.Exp >= skillCost;
                
            foreach (var card in _skillCards)
            {
                card.Refresh(expEnough);
            }
        }

        private void OnLearnSkill(Skills skill)
        {
            SoundManager.Instance.PlaySound(UIActionType.Unlock);
            PlayerAPI.Data.Skills.Add(skill);
            onStartTraining.Invoke(() => skillCost);
        }

        private void OnDestroy()
        {
            foreach (var card in _skillCards)
            {
                card.onUnlock -= OnLearnSkill;
            }
            
            _skillCards.Clear();
        }
    }
}