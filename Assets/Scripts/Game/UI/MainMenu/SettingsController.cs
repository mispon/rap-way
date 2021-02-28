using Core;
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
        [SerializeField] private Button saveButton;

        private void Start()
        {
            langCarousel.onChange += OnLangChanged;
            soundVolume.onValueChanged.AddListener(OnVolumeChanged);
            musicVolume.onValueChanged.AddListener(OnVolumeChanged);
            saveButton.onClick.AddListener(SaveSettings);

            SetupControls();
        }

        /// <summary>
        /// Устанавливает значения контролов из текущих настроек
        /// </summary>
        private void SetupControls()
        {
            var settings = GameManager.Instance.GameStats;
            langCarousel.SetIndex(LangToString(settings.Lang));
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
            var settings = GameManager.Instance.GameStats;
            settings.Lang = StringToLang(langCarousel.GetLabel());
            settings.SoundVolume = soundVolume.value;
            settings.MusicVolume = musicVolume.value;
            MainMenuController.SetPanelActivity(gameObject, false);
        }

        /// <summary>
        /// Маппит перечисление языков на строковое представление
        /// </summary>
        private static string LangToString(SystemLanguage value)
        {
            switch (value)
            {
                case SystemLanguage.Russian:
                    return "RU";
                case SystemLanguage.English:
                    return "EN";
                default:
                    throw new RapWayException($"LangToString: Не найден матчинг для {value}!");
            }
        }

        /// <summary>
        /// Маппит строковое представление на перечисление языков
        /// </summary>
        private static SystemLanguage StringToLang(string value)
        {
            switch (value)
            {
                case "RU":
                    return SystemLanguage.Russian;
                case "EN":
                    return SystemLanguage.English;
                default:
                    throw new RapWayException($"StringToLang: Не найден матчинг для {value}!");
            }
        }
    }
}