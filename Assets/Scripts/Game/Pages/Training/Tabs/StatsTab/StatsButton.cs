using System;
using Data;
using Localization;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs.StatsTab
{
    /// <summary>
    /// Кнопка отображения параметра
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class StatsButton : MonoBehaviour
    {
        [SerializeField] private Text header;
        [SerializeField] private Text desc;
        [Space, SerializeField] private GameObject levelIcon;
        [SerializeField] private Text level;
        [Space, SerializeField] private Button upButton;
        [SerializeField] private Slider exp;
        
        private int _index;

        public Action<int> onClick = index => {};
        public Action<int> onLevelUpClick = index => {};

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(HandleClick);
            upButton.onClick.AddListener(HandleLevelUpClick);
        }

        /// <summary>
        /// Устанавливает индекс элемента 
        /// </summary>
        public void SetIndex(int index)
        {
            _index = index;
        }

        /// <summary>
        /// Раскрывает карточку навыка 
        /// </summary>
        public void Show(PlayerStatsInfo info, bool expEnough, int expToUp)
        {
            var stat = PlayerManager.Data.Stats.Values[_index];
            
            header.text = LocalizationManager.Instance.Get(info.NameKey);
            desc.text = LocalizationManager.Instance.Get(info.DescriptionKey);
            
            level.text = stat.Value.ToString();
            levelIcon.SetActive(true);

            bool noLimit = stat.Value < PlayerData.MAX_SKILL;
            upButton.interactable = expEnough;
            upButton.gameObject.SetActive(noLimit);
            exp.gameObject.SetActive(noLimit);
            
            exp.maxValue = expToUp;
            exp.value = stat.Exp;
        }

        /// <summary>
        /// Скрывает карточку навыка
        /// </summary>
        public void Hide()
        {
            header.text = "";
            desc.text = "";
            level.text = "";
            
            levelIcon.SetActive(false);
            upButton.gameObject.SetActive(false);
            
            exp.gameObject.SetActive(false);
        }

        /// <summary>
        /// Обработчик клика по карточке навыка
        /// </summary>
        private void HandleClick()
        {
            onClick.Invoke(_index);
        }

        /// <summary>
        /// Обработчик клика по апгрейду навыка
        /// </summary>
        private void HandleLevelUpClick()
        {
            onLevelUpClick.Invoke(_index);
        }
    }
}