using System;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Форма с детальной информацией о стилистике
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class TrainingToneView<T> : MonoBehaviour
        where T : Enum
    {
        [SerializeField] private Text toneName;
        [SerializeField] private Button unlockButton;

        private T _tone;
        private Action<T> _onUnlock;

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(Hide);
            unlockButton.onClick.AddListener(UnlockTone);
        }

        /// <summary>
        /// Открывает форму 
        /// </summary>
        public void Show(T tone, bool expEnough, bool isLocked, Action<T> onUnlock)
        {
            string nameKey = tone.GetDescription();
            toneName.text = LocalizationManager.Instance.Get(nameKey);
            
            unlockButton.interactable = expEnough;
            unlockButton.gameObject.SetActive(isLocked);
            
            _tone = tone;
            _onUnlock = onUnlock;
            
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Закрывает форму
        /// </summary>
        private void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Обработчик открытия новой стилистики
        /// </summary>
        private void UnlockTone()
        {
            _onUnlock.Invoke(_tone);
            Hide();
        }
    }
}