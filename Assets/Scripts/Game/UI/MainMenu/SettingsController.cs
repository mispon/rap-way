using Core;
using Data;
using Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Carousel;

namespace Game.UI.MainMenu
{
    /// <summary>
    /// Окно настроек игры
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private Carousel langCarousel;
        [SerializeField] private Slider soundVolume;
        [SerializeField] private Slider musicVolume;
        [Space]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button saveButton;

        private void Start()
        {
            langCarousel.onChange += OnLangChanged;
            soundVolume.onValueChanged.AddListener(OnVolumeChanged);
            musicVolume.onValueChanged.AddListener(OnVolumeChanged);
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
            soundVolume.value = settings.SoundVolume;
            musicVolume.value = settings.MusicVolume;
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
        /// Обработчик изменения громкости звука
        /// </summary>
        private void OnVolumeChanged(float volume)
        {
            SoundManager.Instance.SetVolume(soundVolume.value, musicVolume.value);
        }

        /// <summary>
        /// Сохраняет выбранные настройки
        /// </summary>
        private void SaveSettings()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            
            var settings = GameManager.Instance.GameStats;
            settings.Lang = StringToLang(langCarousel.GetLabel());
            settings.SoundVolume = soundVolume.value;
            settings.MusicVolume = musicVolume.value;
            GameManager.Instance.SaveApplicationData();
            
            MainMenuController.SetPanelActivity(gameObject, false);
        }

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