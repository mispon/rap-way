using System.Collections.Generic;
using Core;
using Data;
using Localization;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Utils;
using Utils.Carousel;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Окно настроек игры
    /// </summary>
    public class SettingsController : SerializedMonoBehaviour
    {
        [BoxGroup("Language")] [SerializeField] private Carousel langCarousel;
        
        [BoxGroup("Sound")] [SerializeField] private AudioMixerGroup audioMixerGroup;
        [BoxGroup("Sound")] [SerializeField] private Dictionary<Slider, string> soundGroup;

        [BoxGroup("Buttons")] [SerializeField] private Button closeButton;
        [BoxGroup("Buttons")] [SerializeField] private Button saveButton;

        private void Start()
        {
            langCarousel.onChange += OnLangChanged;
            
            foreach (var group in soundGroup)
                group.Key
                    .OnValueChangedAsObservable()
                    .Subscribe(value => OnChangeVolume(group.Value, value));
            
            closeButton.onClick.AddListener(OnClose);
            saveButton.onClick.AddListener(SaveSettings);

            SetupControls();
        }

        /// <summary>
        /// Устанавливает значения контролов из текущих настроек
        /// </summary>
        private void SetupControls()
        {
            var settings = GameManager.Instance.GameStats;
            langCarousel.SetIndex(settings.Lang.ToString());

            foreach (var group in soundGroup)
                group.Key.value = LoadVolume(group.Value);
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
        /// Сохраняет выбранные настройки
        /// </summary>
        private void SaveSettings()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var settings = GameManager.Instance.GameStats;
            settings.Lang = StringToLang(langCarousel.GetLabel());
            
            foreach (var group in soundGroup)
                SaveVolume(group.Value, group.Key.value);
            
            GameManager.Instance.SaveApplicationData();
            
            MainMenuController.SetPanelActivity(gameObject, false);
        }

        #region SoundSettings
        private void OnChangeVolume(string groupName, float volume)
        {
            audioMixerGroup.audioMixer.SetFloat(groupName, volume);
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
                audioMixerGroup.audioMixer.SetFloat(groupName, maxVolume);
                return maxVolume;
            }
                
            float volume = PlayerPrefs.GetFloat(groupName);
            audioMixerGroup.audioMixer.SetFloat(groupName, volume);
            return volume;
        }
        #endregion

        private void OnClose()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MainMenuController.SetPanelActivity(gameObject, false);
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
    }
}