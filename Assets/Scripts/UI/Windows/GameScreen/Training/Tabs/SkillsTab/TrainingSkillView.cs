using Core;
using Core.Localization;
using Enums;
using Extensions;
using Firebase.Analytics;
using ScriptableObjects;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Training.Tabs.SkillsTab
{
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

        private void Hide()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            gameObject.SetActive(false);
        }

        private void UnlockSkill()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.TrainingOpenSkill);

            _onUnlock.Invoke(_skill);
            Hide();
        }
    }
}