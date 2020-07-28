using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training.Tabs.ToneTab
{
    /// <summary>
    /// Вкладка изучения новых тематик и стилей
    /// </summary>
    public class TrainingToneTab : TrainingTab
    {
        [Header("Элементы вкладки")]
        [SerializeField] private GridLayoutGroup stylesGrid;
        [SerializeField] private TrainingStyleCard styleCardTemplate;
        [SerializeField] private GridLayoutGroup themesGrid;
        [SerializeField] private TrainingThemeCard themeCardTemplate;

        [Header("Данные")]
        [SerializeField] private TrainingInfoData data;

        private List<TrainingStyleCard> _stylesCards;
        private List<TrainingThemeCard> _themesCards;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            CreateStylesCards();
            CreateThemesCards();
        }

        /// <summary>
        /// Создает карточки тренировки стилистик
        /// </summary>
        private void CreateStylesCards()
        {
            var styles = (Styles[]) Enum.GetValues(typeof(Styles));
            _stylesCards = new List<TrainingStyleCard>(styles.Length);

            for (int i = 0; i < styles.Length; i++)
            {
                var card = Instantiate(styleCardTemplate, stylesGrid.transform);
                card.Setup(i, data.StylesInfo.First(e => e.Type == styles[i]));
                card.onUnlock += OnLearnTone;
                
                _stylesCards.Add(card);
            }
        }
        
        /// <summary>
        /// Создает карточки тренировки тематик
        /// </summary>
        private void CreateThemesCards()
        {
            var themes = (Themes[]) Enum.GetValues(typeof(Themes));
            _themesCards = new List<TrainingThemeCard>(themes.Length);

            for (int i = 0; i < themes.Length; i++)
            {
                var card = Instantiate(themeCardTemplate, themesGrid.transform);
                card.Setup(i, data.ThemesInfo.First(e => e.Type == themes[i]));
                card.onUnlock += OnLearnTone;
                
                _themesCards.Add(card);
            }
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            RefreshCards(_stylesCards);
            RefreshCards(_themesCards);
        }

        /// <summary>
        /// Обновляет состояние карточек 
        /// </summary>
        private static void RefreshCards(IEnumerable<TrainingToneCard> cards)
        {
            foreach (var card in cards)
                card.Refresh();
        }

        /// <summary>
        /// Запускает изучение выбранной стилистики
        /// </summary>
        private void OnLearnTone(Enum tone, int cost)
        {
            if (tone is Themes theme)
                PlayerManager.Data.Themes.Add(theme);
            else if (tone is Styles style)
                PlayerManager.Data.Styles.Add(style);

            onStartTraining.Invoke(() => cost);
        }

        private void OnDestroy()
        {
            DisposeCards(_stylesCards);
            DisposeCards(_themesCards);
        }

        /// <summary>
        /// Очищает кэш карточек 
        /// </summary>
        private void DisposeCards<T>(ICollection<T> cards) where T : TrainingToneCard
        {
            foreach (var card in cards)
                card.onUnlock -= OnLearnTone;
            
            cards.Clear();
        }
    }
}