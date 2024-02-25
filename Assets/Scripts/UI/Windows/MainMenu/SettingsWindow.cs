using System.Collections.Generic;
using Core;
using Core.Localization;
using Game;
using UI.Base;
using UI.Controls.Carousel;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class SettingsWindow : CanvasUIElement
    {
        [SerializeField] private Carousel langCarousel;
        [SerializeField] private Dictionary<Slider, string> soundGroup;
        [SerializeField] private Button saveButton;

        private readonly CompositeDisposable _disposable = new();
        
        public override void Show(object ctx)
        {
            var settings = GameManager.Instance.GameStats;
            
            langCarousel.SetIndex(settings.Lang.ToString());
            langCarousel.onChange += OnLangChanged;

            foreach (var group in soundGroup)
            {
                group.Key.value = LoadVolume(group.Value);
                group.Key
                    .OnValueChangedAsObservable()
                    .Subscribe(value => OnChangeVolume(group.Value, value))
                    .AddTo(_disposable);
            }

            saveButton.onClick
                .AsObservable()
                .Subscribe(_ => SaveSettings())
                .AddTo(_disposable);
                
            
            base.Show(ctx);
        }

        public override void Hide()
        {
            langCarousel.onChange -= OnLangChanged;
            _disposable.Clear();
            
            base.Hide();
        }
        
        /// <summary>
        /// Сохраняет выбранные настройки
        /// </summary>
        private void SaveSettings()
        {
            var settings = GameManager.Instance.GameStats;
            settings.Lang = StringToLang(langCarousel.GetLabel());

            foreach (var group in soundGroup)
            {
                SaveVolume(group.Value, group.Key.value);
            }
            
            GameManager.Instance.SaveApplicationData();
        }

        private static void OnChangeVolume(string groupName, float volume)
        {
            SoundManager.Instance.SetVolume(groupName, volume);
        }
        
        private static void SaveVolume(string groupName, float volume)
        {
            PlayerPrefs.SetFloat(groupName, volume);
        }
        
        private static float LoadVolume(string groupName)
        {
            return PlayerPrefs.GetFloat(groupName);
        }

        /// <summary>
        /// Обрабатчик смены языка
        /// </summary>
        private void OnLangChanged(int index)
        {
            var lang = StringToLang(langCarousel.GetLabel());
            LocalizationManager.Instance.LoadLocalization(lang, true);
        }
        
        /// <summary>
        /// Маппит строковое представление на перечисление языков
        /// </summary>
        private static GameLang StringToLang(string value)
        {
            return value switch
            {
                "RU" => GameLang.RU,
                "EN" => GameLang.EN,
                "DE" => GameLang.DE,
                "FR" => GameLang.FR,
                "IT" => GameLang.IT,
                "ES" => GameLang.ES,
                "PT" => GameLang.PT,
                _ => throw new RapWayException($"StringToLang: Не найден матчинг для {value}!")
            };
        }
    }
}