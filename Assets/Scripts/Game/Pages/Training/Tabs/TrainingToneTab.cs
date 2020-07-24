using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;

namespace Game.Pages.Training.Tabs
{
    /// <summary>
    /// Вкладка изучения новых тематик и стилей
    /// </summary>
    public class TrainingToneTab : TrainingTab
    {
        [Header("Цена покупки новой стилистики")]
        [SerializeField] private int toneCost = 200;
        
        [Header("Контролы")]
        [SerializeField] private Switcher themesSwitcher;
        [SerializeField] private Switcher stylesSwitcher;
        [SerializeField] private Text noThemeLabel;
        [Space]
        [SerializeField] private Button learnThemeButton;
        [SerializeField] private Button learnStyleButton;
        [SerializeField] private Text noStyleLabel;

        private Themes _selectedTheme;
        private Styles _selectedStyle;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            themesSwitcher.onIndexChange += OnThemeChanged;
            stylesSwitcher.onIndexChange += OnStyleChanged;
            learnThemeButton.onClick.AddListener(() => OnLearnTone(LearnThemeCallback));
            learnStyleButton.onClick.AddListener(() => OnLearnTone(LearnStyleCallback));
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            RefreshSwitcher(themesSwitcher, PlayerManager.Data.Themes);
            RefreshSwitcher(stylesSwitcher, PlayerManager.Data.Styles);
        }

        /// <summary>
        /// Обновляет значения свитчера
        /// </summary>
        private void RefreshSwitcher<T>(Switcher switcher, ICollection<T> playerData) where T : Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            
            var displayData = values
                .Where(e => !playerData.Contains(e))
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
            
            if (displayData.Length == 0)
                ShowEmptyLabel<T>();
            
            switcher.InstantiateElements(displayData);
            switcher.ResetActive();
        }

        /// <summary>
        /// Скрывает контролы и показывает сообщение о том, что все открыто и изучено
        /// </summary>
        private void ShowEmptyLabel<T>() where T : Enum
        {
            bool isTheme = typeof(T) == typeof(Themes);
            
            (isTheme ? themesSwitcher : stylesSwitcher).gameObject.SetActive(false);
            (isTheme ? learnThemeButton : learnStyleButton).gameObject.SetActive(false);
            (isTheme ? noThemeLabel : noStyleLabel).gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Обработчик изменения тематики
        /// </summary>
        private void OnThemeChanged(int index)
        {
            var key = LocalizationManager.Instance.GetKey(themesSwitcher.ActiveTextValue);
            var theme = EnumExtension.GetFromDescription<Themes>(key);

            _selectedTheme = theme;
        }
        
        /// <summary>
        /// Обработчик изменения стилистики
        /// </summary>
        private void OnStyleChanged(int index)
        {
            var key = LocalizationManager.Instance.GetKey(stylesSwitcher.ActiveTextValue);
            var style = EnumExtension.GetFromDescription<Styles>(key);

            _selectedStyle = style;
        }

        /// <summary>
        /// Запускает изучение выбранной стилистики
        /// </summary>
        private void OnLearnTone(Func<int> trainingAction)
        {
            onStartTraining.Invoke(trainingAction);
        }

        /// <summary>
        /// Коллбэк завершения изучения новой тематики
        /// </summary>
        private int LearnThemeCallback()
        {
            PlayerManager.Data.Themes.Add(_selectedTheme);
            return toneCost;
        }
        
        /// <summary>
        /// Коллбэк завершения изучения новой стилистики
        /// </summary>
        private int LearnStyleCallback()
        {
            PlayerManager.Data.Styles.Add(_selectedStyle);
            return toneCost;
        }

        private void OnDestroy()
        {
            themesSwitcher.onIndexChange -= OnThemeChanged;
            stylesSwitcher.onIndexChange -= OnStyleChanged;
        }
    }
}