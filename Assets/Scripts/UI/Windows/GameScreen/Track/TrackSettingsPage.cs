using System;
using System.Linq;
using Core;
using Core.Events;
using Core.Localization;
using Enums;
using Extensions;
// using Firebase.Analytics;
using Game.Player.State.Desc;
using Game.Player.Team;
using Game.Production;
using MessageBroker;
using MessageBroker.Messages.UI;
using Models.Production;
using Models.Trends;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.Carousel;
using UI.Enums;
using UI.Windows.Tutorial;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Track
{
    public class TrackSettingsPage : Page
    {
        [BoxGroup("Controls"), SerializeField] private InputField trackNameInput;
        [BoxGroup("Controls"), SerializeField] private Carousel styleCarousel;
        [BoxGroup("Controls"), SerializeField] private Carousel themeCarousel;
        [BoxGroup("Controls"), SerializeField] private Button startButton;
        [BoxGroup("Controls"), SerializeField] protected Text bitSkill;
        [BoxGroup("Controls"), SerializeField] protected Text textSkill;
        [BoxGroup("Controls"), SerializeField] private Image bitmakerAvatar;
        [BoxGroup("Controls"), SerializeField] private Image textwritterAvatar;
        [BoxGroup("Controls"), SerializeField] private GameObject backButton;

        [BoxGroup("Images"), SerializeField] private ImagesBank imagesBank;
        
        protected TrackInfo _track;

        private void Start()
        {
            trackNameInput.onValueChanged.AddListener(OnTrackNameInput);
            startButton.onClick.AddListener(CreateTrack);
        }

        protected override void AfterShow(object ctx = null)
        {
            HintsManager.Instance.ShowHint("tutorial_track_page");
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewTrackSelected);
        }

        private void OnTrackNameInput(string value)
        {
            _track.Name = value;
        }

        private void CreateTrack()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);

            _track.Id = ProductionManager.GetNextProductionId<TrackInfo>();
            if (string.IsNullOrEmpty(_track.Name))
            {
                _track.Name = $"Track {_track.Id}";
            }

            _track.TrendInfo = new TrendInfo
            {
                Style = styleCarousel.GetValue<Styles>(),
                Theme = themeCarousel.GetValue<Themes>()
            };
            
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = _track.Feat == null 
                    ? WindowType.ProductionTrackWork 
                    : WindowType.ProductionFeatWork,
                Context = _track
            });
        }

        protected void SetupCarousel(PlayerData data)
        {
            var styleProps = data.Styles.Select(ConvertToCarouselProps).ToArray();
            styleCarousel.Init(styleProps);
            var themeProps = data.Themes.Select(ConvertToCarouselProps).ToArray();
            themeCarousel.Init(themeProps);
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

        private void ResetTeam(object[] args)
        {
            bitmakerAvatar.sprite    = imagesBank.BitmakerInactive;
            textwritterAvatar.sprite = imagesBank.TextwritterInactive;

            var playerStats = PlayerAPI.Data.Stats;
            bitSkill.text   = $"{playerStats.Bitmaking.Value}";
            textSkill.text  = $"{playerStats.Vocobulary.Value}";
        }

        private CarouselProps ConvertToCarouselProps<T>(T value) where T : Enum
        {
            string text = LocalizationManager.Instance.Get(value.GetDescription());
            Sprite icon = value.GetType() == typeof(Themes)
                ? imagesBank.ThemesActive[Convert.ToInt32(value)]
                : imagesBank.StyleActive;

            return new CarouselProps {Text = text, Sprite = icon, Value = value};
        }

        protected override void BeforeShow(object ctx = null)
        {
            _track = new TrackInfo();
            
            SetupCarousel(PlayerAPI.Data);
            SetupTeam();
            DisplaySkills(PlayerAPI.Data);
            
            bool hasAnyTracks = PlayerAPI.Data.History.TrackList.Count > 0;
            backButton.SetActive(hasAnyTracks);
            
            EventManager.AddHandler(EventType.UncleSamsParty, ResetTeam);
        }

        protected override void AfterHide()
        {
            EventManager.RemoveHandler(EventType.UncleSamsParty, ResetTeam);

            _track = null;
            trackNameInput.SetTextWithoutNotify(string.Empty);
        }
    }
}