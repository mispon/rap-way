using System;
using System.Collections.Generic;
using System.Linq;
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
        [Header("Цена покупки новой стилистики")]
        [SerializeField] private int toneCost = 200;
        
        [Header("Элементы вкладки")]
        [SerializeField] private GridLayoutGroup themesGrid;
        [SerializeField] private GridLayoutGroup stylesGrid;
        [SerializeField] private TrainingToneCard cardTemplate;

        private List<TrainingToneCard> _toneCards;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            var themes = Enum.GetValues(typeof(Themes)).Cast<Enum>().ToArray();
            var styles = Enum.GetValues(typeof(Styles)).Cast<Enum>().ToArray();
            
            _toneCards = new List<TrainingToneCard>(themes.Length + styles.Length);

            CreateCards(themesGrid, in themes);
            CreateCards(stylesGrid, in styles);
        }

        /// <summary>
        /// Создает тренировочные карточки 
        /// </summary>
        private void CreateCards(GridLayoutGroup grid, in Enum[] tones)
        {
            for (int i = 0; i < tones.Length; i++)
            {
                var card = Instantiate(cardTemplate, grid.transform);
                
                card.Setup(i, tones[i]);
                card.onUnlock += OnLearnTone;
                
                _toneCards.Add(card);
            }
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            bool expEnough = PlayerManager.Data.Exp >= toneCost;
                
            foreach (var card in _toneCards)
                card.Refresh(expEnough);
        }

        /// <summary>
        /// Запускает изучение выбранной стилистики
        /// </summary>
        private void OnLearnTone<T>(T tone) where T : Enum
        {
            object t = tone;
            if (typeof(T) == typeof(Themes))
                PlayerManager.Data.Themes.Add((Themes) t);
            else
                PlayerManager.Data.Styles.Add((Styles) t);
            
            onStartTraining.Invoke(() => toneCost);
        }

        private void OnDestroy()
        {
            foreach (var card in _toneCards)
                card.onUnlock -= OnLearnTone;
            
            _toneCards.Clear();
        }
    }
}