using System.Collections.Generic;
using Core;
using Core.Localization;
using Game;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Controls.Carousel;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Windows.MainMenu
{
    public class SettingsWindow : CanvasUIElement
    {
        [BoxGroup("Language")] [SerializeField] private Carousel _langCarousel;
        
        [BoxGroup("Sound")] [SerializeField] private AudioMixerGroup _audioMixerGroup;
        [BoxGroup("Sound")] [SerializeField] private Dictionary<Slider, string> _soundGroup;

        [BoxGroup("Buttons")] [SerializeField] private Button _saveButton;

        public override void Initialize()
        {
            base.Initialize();
            
            var settings = GameManager.Instance.GameStats;
            _langCarousel.SetIndex(settings.Lang.ToString());

            foreach (var group in _soundGroup)
                group.Key.value = LoadVolume(group.Value);
        }

        protected override void SetupListenersOnInitialize()
        {
            base.SetupListenersOnInitialize();
            
            _langCarousel.onChange += OnLangChanged;

            foreach (var group in _soundGroup)
                group.Key
                    .OnValueChangedAsObservable()
                    .Subscribe(value => OnChangeVolume(group.Value, value));
            
            _saveButton.onClick.AddListener(SaveSettings);
        }
        
        /// <summary>
        /// Сохраняет выбранные настройки
        /// </summary>
        private void SaveSettings()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var settings = GameManager.Instance.GameStats;
            settings.Lang = StringToLang(_langCarousel.GetLabel());
            
            foreach (var group in _soundGroup)
                SaveVolume(group.Value, group.Key.value);
            
            GameManager.Instance.SaveApplicationData();
        }

        #region SoundSettings
        private void OnChangeVolume(string groupName, float volume)
        {
            _audioMixerGroup.audioMixer.SetFloat(groupName, volume);
        }
        
        private void SaveVolume(string groupName, float volume)
        {
            PlayerPrefs.SetFloat(groupName, volume);
        }
        
        private float LoadVolume(string groupName)
        {
            if (PlayerPrefs.HasKey(groupName) is false)
            {
                const float maxVolume = 0;
                SaveVolume(groupName, maxVolume);
                _audioMixerGroup.audioMixer.SetFloat(groupName, maxVolume);
                return maxVolume;
            }
                
            float volume = PlayerPrefs.GetFloat(groupName);
            _audioMixerGroup.audioMixer.SetFloat(groupName, volume);
            return volume;
        }
        #endregion

        #region LanguageSettings
        /// <summary>
        /// Обрабатчик смены языка
        /// </summary>
        private void OnLangChanged(int index)
        {
            var lang = StringToLang(_langCarousel.GetLabel());
            LocalizationManager.Instance.LoadLocalization(lang, true);
        }
        
        /// <summary>
        /// Маппит строковое представление на перечисление языков
        /// </summary>
        private static GameLang StringToLang(string value)
        {
            switch (value)
            {
                case "RU":
                    return GameLang.RU;
                case "EN":
                    return GameLang.EN;
                case "DE":
                    return GameLang.DE;
                case "FR":
                    return GameLang.FR;
                case "IT":
                    return GameLang.IT;
                case "ES":
                    return GameLang.ES;
                case "PT":
                    return GameLang.PT;
                default:
                    throw new RapWayException($"StringToLang: Не найден матчинг для {value}!");
            }
        }
        #endregion
    }
}