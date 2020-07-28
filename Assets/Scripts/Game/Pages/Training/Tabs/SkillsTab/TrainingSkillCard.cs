using System;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs.SkillsTab
{
    /// <summary>
    /// Карточка умения в гриде умений персонажа
    /// </summary>
    public class TrainingSkillCard : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TrainingSkillView view;

        public Action<Skills> onUnlock = skill => {};
        
        private PlayerSkillInfo _info;
        private bool _expEnough;
        private bool _locked;

        private void Start()
        {
            button.onClick.AddListener(ShowInfo);
        }

        /// <summary>
        /// Устанавливает данные карточки
        /// </summary>
        public void Setup(int index, PlayerSkillInfo info)
        {
            _info = info;

            name = $"SkillCard-{index + 1}";
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Обновляет состояние карточки
        /// </summary>
        public void Refresh(bool expEnough)
        {
            _expEnough = expEnough;
            _locked = !PlayerManager.Data.Skills.Contains(_info.Type);
            
            button.image.sprite = _locked ? _info.Locked : _info.Normal;
        }

        /// <summary>
        /// Показывает информацию об умении
        /// </summary>
        private void ShowInfo()
        {
            view.Show(_info, _expEnough, _locked, onUnlock);
        }
    }
}