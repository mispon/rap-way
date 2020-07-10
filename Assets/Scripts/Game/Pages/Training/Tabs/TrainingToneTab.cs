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
        [Header("Контролы")]
        [SerializeField] private Switcher themesSwitcher;
        [SerializeField] private Switcher stylesSwitcher;
        [SerializeField] private Button learnThemeButton;
        [SerializeField] private Button learnStyleButton;

        private Themes _selectedTheme;
        private Styles _selectedStyle;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            themesSwitcher.onIndexChange += OnThemeChanged;
            stylesSwitcher.onIndexChange += OnStyleChanged;
            learnThemeButton.onClick.AddListener(OnLearnTone<Themes>);
            learnStyleButton.onClick.AddListener(OnLearnTone<Styles>);
        }
        
        /// <summary>
        /// Активирует / деактивирует вкладку
        /// </summary>
        public override void Toggle(bool isOpen)
        {
            if (isOpen)
            {
                RefreshSwitcher(themesSwitcher, PlayerManager.Data.Themes);
                RefreshSwitcher(stylesSwitcher, PlayerManager.Data.Styles);
                
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Обновляет значения свитчера
        /// </summary>
        private static void RefreshSwitcher<T>(Switcher switcher, ICollection<T> playerData) where T : Enum
        {
            var values = (T[]) Enum.GetValues(typeof(T));
            
            var displayData = values
                .Where(e => !playerData.Contains(e))
                .Select(e => LocalizationManager.Instance.Get(e.GetDescription()))
                .ToArray();
            
            switcher.InstantiateElements(displayData);
            switcher.ResetActive();
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
        private void OnLearnTone<T>() where T : Enum
        {
            Action callback;
            if (typeof(T) == typeof(Themes))
                callback = () => PlayerManager.Data.Themes.Add(_selectedTheme);
            else
                callback = () => PlayerManager.Data.Styles.Add(_selectedStyle);
            
            var onFinish = new Action(callback);
            onStartTraining.Invoke(trainingDuration, onFinish);
        }

        private void OnDestroy()
        {
            themesSwitcher.onIndexChange -= OnThemeChanged;
            stylesSwitcher.onIndexChange -= OnStyleChanged;
        }
    }
}