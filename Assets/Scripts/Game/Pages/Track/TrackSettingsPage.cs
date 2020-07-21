using Enums;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Track
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

        protected TrackInfo _track;

        private void Start()
        {
            trackNameInput.onValueChanged.AddListener(OnTrackNameInput);
            startButton.onClick.AddListener(CreateTrack);
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
            _track.Id = PlayerManager.GetNextProductionId<TrackInfo>();
            if (string.IsNullOrEmpty(_track.Name))
            {
                _track.Name = $"Track {_track.Id}";
            }
            _track.Theme = GetToneValue<Themes>(themeSwitcher);
            _track.Style = GetToneValue<Styles>(styleSwitcher);
            
            workingPage.CreateTrack(_track);
            Close();
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            _track = new TrackInfo();
            
            themeSwitcher.InstantiateElements(PlayerManager.GetPlayersThemes());
            styleSwitcher.InstantiateElements(PlayerManager.GetPlayersStyles());
        }

        protected override void AfterPageClose()
        {
            _track = null;
            
            trackNameInput.SetTextWithoutNotify(string.Empty);
            themeSwitcher.ResetActive();
            styleSwitcher.ResetActive();
        }

        #endregion
    }
}