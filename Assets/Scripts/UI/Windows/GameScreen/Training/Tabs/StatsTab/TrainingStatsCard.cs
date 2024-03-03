using System;
using System.Collections.Generic;
using Core;
using Core.Localization;
using Enums;
using Firebase.Analytics;
using Game.Player;
using Game.Player.State.Desc;
using ScriptableObjects;
using UI.Controls.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Training.Tabs.StatsTab
{
    /// <summary>
    /// Кнопка отображения параметра
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class TrainingStatsCard : MonoBehaviour
    {
        [SerializeField] private Text header;
        [SerializeField] private Text desc;
        [SerializeField] private Text level;
        [SerializeField] private Button upButton;
        [SerializeField] private ProgressBar expBar;

        [Header("Фон карточки")]
        [SerializeField] private Image cardImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite darkSprite;
        
        private int _index;

        public Action<int> onClick = index => {};
        public Action<int> onLevelUpClick = index => {};

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
            upButton.onClick.AddListener(HandleLevelUpClick);

            header.text = string.Empty;
            upButton.gameObject.SetActive(false);
            expBar.gameObject.SetActive(false);
        }

        /// <summary>
        /// Устанавливает индекс элемента 
        /// </summary>
        public void SetIndex(int index)
        {
            _index = index;
            DisplayLevel();
        }

        /// <summary>
        /// Раскрывает карточку навыка 
        /// </summary>
        public void Show(PlayerStatsInfo info, bool expEnough, int expToUp)
        {
            var stat = PlayerManager.Data.Stats.Values[_index];
            
            header.text = LocalizationManager.Instance.Get(info.NameKey);
            desc.text = LocalizationManager.Instance.Get(info.DescriptionKey);
            
            DisplayLevel();

            bool noLimit = stat.Value < PlayerData.MAX_SKILL;
            upButton.interactable = expEnough;
            upButton.gameObject.SetActive(noLimit);
            
            expBar.gameObject.SetActive(noLimit);
            expBar.SetValue(stat.Exp, expToUp);

            cardImage.sprite = darkSprite;
        }

        /// <summary>
        /// Скрывает карточку навыка
        /// </summary>
        public void Hide()
        {
            header.text = string.Empty;
            desc.text = string.Empty;

            DisplayLevel();

            upButton.gameObject.SetActive(false);
            expBar.gameObject.SetActive(false);

            cardImage.sprite = normalSprite;
        }

        /// <summary>
        /// Отображает текущий уровень навыка
        /// </summary>
        private void DisplayLevel()
        {
            int value = PlayerManager.Data.Stats.Values[_index].Value;
            level.text = value.ToString();
        }

        /// <summary>
        /// Обработчик клика по карточке навыка
        /// </summary>
        private void HandleClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            onClick.Invoke(_index);
        }

        /// <summary>
        /// Обработчик клика по апгрейду навыка
        /// </summary>
        private void HandleLevelUpClick()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            SendFirebaseEvent();
            
            onLevelUpClick.Invoke(_index);
        }

        private void SendFirebaseEvent()
        {
            var eventsMap = new Dictionary<int, string>
            {
                [0] = FirebaseGameEvents.TrainingVocabularyUpgrade,
                [1] = FirebaseGameEvents.TrainingBitmakingUpgrade,
                [2] = FirebaseGameEvents.TrainingFlowUpgrade,
                [3] = FirebaseGameEvents.TrainingCharismaUpgrade,
                [4] = FirebaseGameEvents.TrainingManagementUpgrade,
                [5] = FirebaseGameEvents.TrainingMarketingUpgrade
            };
            
            FirebaseAnalytics.LogEvent(eventsMap[_index]);
        }
    }
}