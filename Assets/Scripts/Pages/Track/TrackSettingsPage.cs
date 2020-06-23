using System;
using System.Linq;
using Core;
using Enums;
using Localization;
using Models.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Pages.Track
{
    /// <summary>
    /// Страница настройки трека
    /// </summary>
    public class TrackSettingsPage : Page
    {
        [Header("Контроллы")]
        [SerializeField] private InputField trackNameInput;
        [SerializeField] private Switcher themeSwitcher;
        [SerializeField] private Switcher styleSwitcher;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")]
        [SerializeField] private TrackWorkingPage workingPage;

        private TrackInfo _track;

        private void Start()
        {
            trackNameInput.onValueChanged.AddListener(OnTrackNameInput);
            startButton.onClick.AddListener(CreateTrack);

            var themes = EnumExtension.GetDiscriptions<Themes>()
                .Select(e => LocalizationManager.Instance.Get(e));
            themeSwitcher.InstantiateElements(themes);

            var styles = EnumExtension.GetDiscriptions<Styles>()
                .Select(e => LocalizationManager.Instance.Get(e));
            styleSwitcher.InstantiateElements(styles);
        }
        
        /// <summary>
        /// Обработчик ввода названия трека 
        /// </summary>
        private void OnTrackNameInput(string value)
        {
            _track.Name = value;
        }

        /// <summary>
        /// Обработчик запуска работы над треком
        /// </summary>
        private void CreateTrack()
        {
            _track.Id = PlayerManager.GetNextId<TrackInfo>();

            if (string.IsNullOrEmpty(_track.Name))
                _track.Name = $"Track {_track.Id}";

            _track.Theme = (Themes) themeSwitcher.ActiveIndex;
            _track.Style = (Styles) styleSwitcher.ActiveIndex;
            
            workingPage.CreateTrack(_track);
            Close();
        }

        protected override void BeforePageOpen()
        {
            _track = new TrackInfo();
        }

        protected override void AfterPageClose()
        {
            _track = null;
            
            trackNameInput.SetTextWithoutNotify(string.Empty);
            themeSwitcher.ResetActive();
            styleSwitcher.ResetActive();
        }
    }
}