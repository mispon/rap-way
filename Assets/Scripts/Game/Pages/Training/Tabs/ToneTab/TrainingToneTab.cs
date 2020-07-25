using System;
using System.Collections.Generic;
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
        
        [Header("Элемнты вкладки")]
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private GameObject cardTemplate;

        private List<TrainingToneCard<Themes>> _themesCards;
        private List<TrainingToneCard<Styles>> _stylesCards;

        /// <summary>
        /// Инициализация вкладки
        /// </summary>
        public override void Init()
        {
            CreateCards(out _themesCards);
            CreateCards(out _stylesCards);
        }

        /// <summary>
        /// Вызывается при открытии
        /// </summary>
        protected override void OnOpen()
        {
            bool expEnough = PlayerManager.Data.Exp >= toneCost;
                
            foreach (var card in _themesCards)
                card.Refresh(expEnough);
            
            foreach (var card in _stylesCards)
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

        /// <summary>
        /// Заполняет грид карточками стилистик
        /// </summary>
        private void CreateCards<T>(out List<TrainingToneCard<T>> cards)
            where T : Enum
        {
            var tones = (T[]) Enum.GetValues(typeof(T));
            cards = new List<TrainingToneCard<T>>(tones.Length);

            for (int i = 0; i < tones.Length; i++)
            {
                var cardObject = Instantiate(cardTemplate, grid.transform);
                var card = cardObject.GetComponent<TrainingToneCard<T>>();
                
                card.Setup(i, tones[i]);
                card.onUnlock += OnLearnTone;
                
                cards.Add(card);
            }
        }

        private void OnDestroy()
        {
            DisposeCards(_themesCards);
            DisposeCards(_stylesCards);
        }

        /// <summary>
        /// Очищает кэш карточек 
        /// </summary>
        private void DisposeCards<T>(List<TrainingToneCard<T>> cards)
            where T : Enum
        {
            foreach (var card in cards)
                card.onUnlock -= OnLearnTone;
            
            cards.Clear();
        }
    }
}