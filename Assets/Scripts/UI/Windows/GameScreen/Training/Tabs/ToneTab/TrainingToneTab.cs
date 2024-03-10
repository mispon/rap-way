using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Training.Tabs.ToneTab
{
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

        public override void Init()
        {
            CreateStylesCards();
            CreateThemesCards();
        }

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

        protected override void OnOpen()
        {
            RefreshCards(_stylesCards);
            RefreshCards(_themesCards);
        }

        private static void RefreshCards(IEnumerable<TrainingToneCard> cards)
        {
            foreach (var card in cards)
                card.Refresh();
        }

        private void OnLearnTone(Enum tone, int cost)
        {
            SoundManager.Instance.PlaySound(UIActionType.Unlock);
            
            switch (tone)
            {
                case Themes theme:
                    PlayerAPI.Data.Themes.Add(theme);
                    break;
                case Styles style:
                    PlayerAPI.Data.Styles.Add(style);
                    break;
            }
            
            onStartTraining.Invoke(() => cost);
        }

        private void OnDestroy()
        {
            DisposeCards(_stylesCards);
            DisposeCards(_themesCards);
        }

        private void DisposeCards<T>(ICollection<T> cards) where T : TrainingToneCard
        {
            foreach (var card in cards)
                card.onUnlock -= OnLearnTone;
            
            cards.Clear();
        }
    }
}