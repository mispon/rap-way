using System;
using System.Linq;
using Data;
using Enums;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs
{    
    /// <summary>
    /// Вкладка тренировки умений
    /// </summary>
    public class TrainingSkillsTab : TrainingTab
    {
        [Header("Контролы")]
        [SerializeField] private Switcher skillsSwitcher;
        [SerializeField] private Text header;
        [SerializeField] private Text desc;
        [SerializeField] private Button learnButton;

        [Header("Данные")]
        [SerializeField] private TrainingInfoData data;

        private Skills _selectedSkill;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            skillsSwitcher.onIndexChange += OnSkillChanged;
            learnButton.onClick.AddListener(OnLearnSkill);
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            RefreshSwitcher();
        }

        /// <summary>
        /// Заполняет свитчер актуальными значениями
        /// </summary>
        private void RefreshSwitcher()
        {
            var skills = (Skills[]) Enum.GetValues(typeof(Skills));
            var values = skills
                .Where(e => !PlayerManager.Data.Skills.Contains(e))
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
            
            skillsSwitcher.InstantiateElements(values);
            skillsSwitcher.ResetActive();
        }
        
        /// <summary>
        /// Обработчик смены умения 
        /// </summary>
        private void OnSkillChanged(int index)
        {
            var key = LocalizationManager.Instance.GetKey(skillsSwitcher.ActiveTextValue);
            var skill = EnumExtension.GetFromDescription<Skills>(key);
            
            var item = data.SkillsInfo.First(e => e.Type == skill);
            header.text = skillsSwitcher.ActiveTextValue;
            desc.text = Locale(item.DescriptionKey);

            _selectedSkill = skill;
        }

        /// <summary>
        /// Обработчик изучения нового умения
        /// </summary>
        private void OnLearnSkill()
        {
            string OnFinish()
            {
                PlayerManager.Data.Skills.Add(_selectedSkill);
                return $"{Locale("training_newSkill")}: {Locale(_selectedSkill.GetDescription())}";
            }

            onStartTraining.Invoke(trainingDuration, OnFinish);
        }

        private void OnDestroy()
        {
            skillsSwitcher.onIndexChange -= OnSkillChanged;
        }
    }
}