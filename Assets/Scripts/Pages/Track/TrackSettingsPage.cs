using Models.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Pages.Track
{
    /// <summary>
    /// Страница настройки трека
    /// </summary>
    public class TrackSettingsPage : Page
    {
        [Header("Контроллы")]
        [SerializeField] private InputField trackNameInput;
        [SerializeField] private Dropdown themeDropdown;
        [SerializeField] private Dropdown styleDropdown;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")]
        [SerializeField] private TrackWorkingPage workingPage;

        private TrackInfo _track;

        private void Start()
        {
            trackNameInput.onValueChanged.AddListener(OnTrackNameInput);
            themeDropdown.onValueChanged.AddListener(OnThemeChanged);
            styleDropdown.onValueChanged.AddListener(OnStyleChanged);
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
        /// Обработчик смены тематики трека
        /// </summary>
        private void OnThemeChanged(int value)
        {
            print($"Selected {themeDropdown.options[value].text}");
        }
        
        /// <summary>
        /// Обработчик смены стилистики трека
        /// </summary>
        private void OnStyleChanged(int value)
        {
            print($"Selected {styleDropdown.options[value].text}");
        }

        /// <summary>
        /// Обработчик запуска работы над треком
        /// </summary>
        private void CreateTrack()
        {
            // todo: get next id
            _track.Id = 0;
            
            if (string.IsNullOrEmpty(_track.Name))
            {
                _track.Name = $"Track {_track.Id}";
            }
            
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
            
            themeDropdown.SetValueWithoutNotify(0);
            styleDropdown.SetValueWithoutNotify(0);
            trackNameInput.SetTextWithoutNotify(string.Empty);
        }
    }
}