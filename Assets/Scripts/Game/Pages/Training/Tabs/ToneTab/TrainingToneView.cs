using System;
using Core;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Форма с детальной информацией о стилистике
    /// </summary>
    public class TrainingToneView : MonoBehaviour
    {
        [SerializeField] private Text toneName;
        [SerializeField] private Image toneIcon;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button closeButton;

        private ToneViewContext _context;

        private void Start()
        {
            unlockButton.onClick.AddListener(UnlockTone);
            closeButton.onClick.AddListener(Hide);
        }

        /// <summary>
        /// Открывает форму 
        /// </summary>
        public void Show(ToneViewContext context)
        {
            _context = context;
            
            string nameKey = context.Tone.GetDescription();
            toneName.text = LocalizationManager.Instance.Get(nameKey).ToUpper();
            toneIcon.sprite = _context.Icon;
            
            unlockButton.interactable = context.ExpEnough;
            unlockButton.gameObject.SetActive(context.IsLocked);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Закрывает форму
        /// </summary>
        private void Hide()
        {
            SoundManager.Instance.Click();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Обработчик открытия новой стилистики
        /// </summary>
        private void UnlockTone()
        {
            _context.onClick.Invoke(_context.Tone, _context.Cost);
            Hide();
        }
    }

    public class ToneViewContext
    {
        public Enum Tone;
        public int Cost;
        public Sprite Icon;
        public bool ExpEnough;
        public bool IsLocked;
        public Action<Enum, int> onClick;
    }
}