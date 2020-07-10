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

        private void Start()
        {
            skillsSwitcher.onIndexChange += OnSkillChanged;
            learnButton.onClick.AddListener(OnLearnSkill);
        }

        /// <summary>
        /// Активирует / деактивирует вкладку
        /// </summary>
        public override void Toggle(bool isOpen)
        {
            if (isOpen)
            {
                RefreshSwitcher();
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
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
            desc.text = item.DescriptionKey; // todo: Localize

            _selectedSkill = skill;
        }

        /// <summary>
        /// Обработчик изучения нового умения
        /// </summary>
        private void OnLearnSkill()
        {
            var onFinish = new Action(() => PlayerManager.Data.Skills.Add(_selectedSkill));
            onStartTraining.Invoke(trainingDuration, onFinish);
        }

        private void OnDestroy()
        {
            skillsSwitcher.onIndexChange -= OnSkillChanged;
        }
    }
}