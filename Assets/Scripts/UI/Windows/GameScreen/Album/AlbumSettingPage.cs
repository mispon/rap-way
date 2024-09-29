using System;
using System.Linq;
using Core;
using Core.Analytics;
using Core.Localization;
using Enums;
using Extensions;
using Game.Player.State.Desc;
using Game.Player.Team;
using Game.Production;
using MessageBroker;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.UI;
using Models.Production;
using Models.Trends;
using ScriptableObjects;
using UI.Controls.Carousel;
using UI.Enums;
using UI.Windows.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Album
{
    public class AlbumSettingPage : Page
    {
        [Header("Controls")]
        [SerializeField] private InputField albumNameInput;
        [SerializeField] private Carousel styleCarousel;
        [SerializeField] private Carousel themeCarousel;
        [SerializeField] private Button startButton;
        [SerializeField] protected Text bitSkill;
        [SerializeField] protected Text textSkill;
        [SerializeField] private Image bitmakerAvatar;
        [SerializeField] private Image textwritterAvatar;

        [Header("Data")]
        [SerializeField] private ImagesBank imagesBank;

        private AlbumInfo _album;
        private IDisposable _disposable;

        private void Start()
        {
            albumNameInput.onValueChanged.AddListener(OnAlbumNameInput);
            startButton.onClick.AddListener(CreateAlbum);
        }

        protected override void AfterShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.NewAlbumSelected);
            HintsManager.Instance.ShowHint("tutorial_album_page");
        }

        private void OnAlbumNameInput(string value)
        {
            _album.Name = value;
        }

        private void CreateAlbum()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            _album.Id = ProductionManager.GetNextProductionId<AlbumInfo>();
            if (string.IsNullOrEmpty(_album.Name))
            {
                _album.Name = $"Album {_album.Id}";
            }

            _album.TrendInfo = new TrendInfo
            {
                Style = styleCarousel.GetValue<Styles>(),
                Theme = themeCarousel.GetValue<Themes>()
            };

            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionAlbumWork,
                Context = _album
            });

            Hide();
        }

        private void SetupCarousel(PlayerData data)
        {
            var styleProps = data.Styles.Select(ConvertToCarouselProps).ToArray();
            styleCarousel.Init(styleProps);

            var themeProps = data.Themes.Select(ConvertToCarouselProps).ToArray();
            themeCarousel.Init(themeProps);
        }

        private CarouselProps ConvertToCarouselProps<T>(T value) where T : Enum
        {
            string text = LocalizationManager.Instance.Get(value.GetDescription());
            Sprite icon = value.GetType() == typeof(Themes)
                ? imagesBank.ThemesActive[Convert.ToInt32(value)]
                : imagesBank.StyleActive;

            return new CarouselProps { Text = text, Sprite = icon, Value = value };
        }

        private void SetupTeam()
        {
            bitmakerAvatar.sprite = TeamManager.IsAvailable(TeammateType.BitMaker)
                ? imagesBank.BitmakerActive
                : imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = TeamManager.IsAvailable(TeammateType.TextWriter)
                ? imagesBank.TextwritterActive
                : imagesBank.TextwritterInactive;
        }

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

        private void ResetTeam()
        {
            bitmakerAvatar.sprite = imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = imagesBank.TextwritterInactive;

            var playerStats = PlayerAPI.Data.Stats;
            bitSkill.text = $"{playerStats.Bitmaking.Value}";
            textSkill.text = $"{playerStats.Vocobulary.Value}";
        }

        protected override void BeforeShow(object ctx = null)
        {
            _album = new AlbumInfo { CreatorId = -1 };

            SetupCarousel(PlayerAPI.Data);
            SetupTeam();
            DisplaySkills(PlayerAPI.Data);

            _disposable = MsgBroker.Instance
                .Receive<TeamSalaryMessage>()
                .Subscribe(e => ResetTeam());
        }

        protected override void AfterHide()
        {
            _disposable?.Dispose();

            _album = null;
            albumNameInput.SetTextWithoutNotify(string.Empty);
        }
    }
}