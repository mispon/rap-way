using System;
using Core;
using Core.Localization;
using Enums;
using Extensions;
// using Firebase.Analytics;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
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
            SoundManager.Instance.PlaySound(UIActionType.Click);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Обработчик открытия новой стилистики
        /// </summary>
        private void UnlockTone()
        {
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrainingOpenTone);
            
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