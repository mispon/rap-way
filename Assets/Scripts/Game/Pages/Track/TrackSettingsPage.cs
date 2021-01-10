using System;
using System.Linq;
using Core;
using Data;
using Enums;
using Game.UI.GameScreen;
using Localization;
using Models.Info;
using Models.Info.Production;
using Models.Player;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Game.Pages.Track
{
    /// <summary>
    /// Страница настройки трека
    /// </summary>
    public class TrackSettingsPage : Page
    {
        [Header("Контроллы")] 
        [SerializeField] private InputField trackNameInput;
        [SerializeField] private Carousel styleCarousel;
        [SerializeField] private Carousel themeCarousel;
        [SerializeField] private Button startButton;
        [Space] 
        [SerializeField] protected Text bitSkill;
        [SerializeField] protected Text textSkill;
        [SerializeField] private Image bitmakerAvatar;
        [SerializeField] private Image textwritterAvatar;

        [Header("Данные")] 
        [SerializeField] private ImagesBank imagesBank;

        [Header("Страница разработки")] 
        [SerializeField]
        private BaseWorkingPage workingPage;

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
            SoundManager.Instance.PlayClick();

            _track.Id = PlayerManager.GetNextProductionId<TrackInfo>();
            if (string.IsNullOrEmpty(_track.Name))
            {
                _track.Name = $"Track {_track.Id}";
            }

            _track.TrendInfo = new TrendInfo
            {
                Style = styleCarousel.GetValue<Styles>(),
                Theme = themeCarousel.GetValue<Themes>()
            };

            workingPage.StartWork(_track);
            Close();
        }

        /// <summary>
        /// Инициализирует карусели актуальными значениями 
        /// </summary>
        protected void SetupCarousel(PlayerData data)
        {
            var styleProps = data.Styles.Select(ConvertToCarouselProps).ToArray();
            styleCarousel.Init(styleProps);
            var themeProps = data.Themes.Select(ConvertToCarouselProps).ToArray();
            themeCarousel.Init(themeProps);
        }

        /// <summary>
        /// Отображает состояние членов команды
        /// </summary>
        private void SetupTeam()
        {
            bitmakerAvatar.sprite = TeamManager.IsAvailable(TeammateType.BitMaker)
                ? imagesBank.BitmakerActive
                : imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = TeamManager.IsAvailable(TeammateType.TextWriter)
                ? imagesBank.TextwritterActive
                : imagesBank.TextwritterInactive;
        }

        /// <summary>
        /// Показывает текущий суммарный скилл команды 
        /// </summary>
        private void DisplaySkills(PlayerData data)
        {
            int playerBitSkill = data.Stats.Bitmaking.Value;
            int bitmakerSkill = TeamManager.IsAvailable(TeammateType.BitMaker)
                ? data.Team.BitMaker.Skill.Value
                : 0;
            bitSkill.text = $"{playerBitSkill + bitmakerSkill}";

            int playerTextSkill = data.Stats.Vocobulary.Value;
            int textwritterSkill = TeamManager.IsAvailable(TeammateType.TextWriter)
                ? data.Team.TextWriter.Skill.Value
                : 0;
            textSkill.text = $"{playerTextSkill + textwritterSkill}";
        }

        /// <summary>
        /// Сбрасывает состояние членов команды и суммарный скилл команды
        /// </summary>
        private void ResetTeam(object[] args)
        {
            bitmakerAvatar.sprite = imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = imagesBank.TextwritterInactive;

            var playerStats = PlayerManager.Data.Stats;
            bitSkill.text = $"{playerStats.Bitmaking.Value}";
            textSkill.text = $"{playerStats.Vocobulary.Value}";
        }

        /// <summary>
        /// Конвертирует элемент перечисление в свойство карусели 
        /// </summary>
        private CarouselProps ConvertToCarouselProps<T>(T value) where T : Enum
        {
            string text = LocalizationManager.Instance.Get(value.GetDescription());
            Sprite icon = value.GetType() == typeof(Themes)
                ? imagesBank.ThemesActive[Convert.ToInt32(value)]
                : imagesBank.StyleActive;

            return new CarouselProps {Text = text, Sprite = icon, Value = value};
        }

        protected override void BeforePageOpen()
        {
            _track = new TrackInfo();

            var data = PlayerManager.Data;
            SetupCarousel(data);
            SetupTeam();
            DisplaySkills(data);

            EventManager.AddHandler(EventType.UncleSamsParty, ResetTeam);
            GameScreenController.Instance.HideProductionGroup();
        }

        protected override void AfterPageClose()
        {
            EventManager.RemoveHandler(EventType.UncleSamsParty, ResetTeam);

            _track = null;
            trackNameInput.SetTextWithoutNotify(string.Empty);
        }
    }
}