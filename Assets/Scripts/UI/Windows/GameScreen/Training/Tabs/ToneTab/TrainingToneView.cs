using Core;
using Core.Localization;
using Enums;
using Extensions;
using Firebase.Analytics;
using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
{
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

        private void Hide()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            gameObject.SetActive(false);
        }

        private void UnlockTone()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrainingOpenTone);

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