using System;
using Data;
using Enums;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs.SkillsTab
{
    /// <summary>
    /// Форма с детальной информацией о навыке
    /// </summary>
    public class TrainingSkillView : MonoBehaviour
    {
        [SerializeField] private Text header;
        [SerializeField] private Text desc;
        [SerializeField] private Image skillImage;
        [SerializeField] private Button viewButton;
        [SerializeField] private Button unlockButton;

        private Skills _skill;
        private Action<Skills> _onUnlock;

        private void Start()
        {
            viewButton.onClick.AddListener(Hide);
            unlockButton.onClick.AddListener(UnlockSkill);
        }

        /// <summary>
        /// Показывает форму с информацией об умении
        /// </summary>
        public void Show(PlayerSkillInfo info, bool expEnough, bool isLocked, Action<Skills> onUnlock)
        {
            var locale = LocalizationManager.Instance;
            
            header.text = locale.Get(info.Type.GetDescription()).ToUpper();
            desc.text = locale.Get(info.DescriptionKey);

            unlockButton.interactable = expEnough;
            unlockButton.gameObject.SetActive(isLocked);

            _skill = info.Type;
            _onUnlock = onUnlock;

            skillImage.sprite = info.Normal;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Скрывает форму
        /// </summary>
        private void Hide()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Обработчик кнопки разблокировки умения
        /// </summary>
        private void UnlockSkill()
        {
            _onUnlock.Invoke(_skill);
            Hide();
        }
    }
}