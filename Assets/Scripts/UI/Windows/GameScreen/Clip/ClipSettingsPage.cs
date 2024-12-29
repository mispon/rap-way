using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Analytics;
using Enums;
using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using UI.Controls.Carousel;
using UI.Controls.Error;
using UI.Controls.Money;
using UI.Enums;
using UI.Windows.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using PlayerAPI = Game.Player.PlayerPackage;

namespace UI.Windows.GameScreen.Clip
{
    public class ClipSettingsPage : Page
    {
        [Header("Controls")]
        [SerializeField] private Carousel trackCarousel;
        [SerializeField] private Carousel directorCarousel;
        [SerializeField] private Carousel operatorCarousel;
        [SerializeField] private Button   startButton;

        [Header("Labels")]
        [SerializeField] private Text directorSkill;
        [SerializeField] private Text directorPrice;
        [SerializeField] private Text operatorSkill;
        [SerializeField] private Text operatorPrice;

        [Header("Price")]
        [SerializeField] private GameError noMoneyErr;
        [SerializeField] private Price price;

        [Header("Data")]
        [SerializeField] private ClipStaffData staffData;

        private ClipInfo _clip;
        private int      _directorPrice;
        private int      _operatorPrice;

        private const    int             TRACKS_CACHE = 10;
        private readonly List<TrackInfo> _lastTracks  = new(TRACKS_CACHE);

        private readonly CompositeDisposable _disposable = new();

        private int _clipCost => _directorPrice + _operatorPrice;

        private void Start()
        {
            startButton.onClick.AddListener(CreateClip);
            SetupStaffCarousels();
        }

        private void SetupStaffCarousels()
        {
            var directorProps = Enumerable.Range(0, staffData.Directors.Length);
            directorCarousel.Init(directorProps.Select(e => new CarouselProps()).ToArray());
            directorCarousel.onChange += OnDirectorChange;

            var operatorProps = Enumerable.Range(0, staffData.Operators.Length);
            operatorCarousel.Init(operatorProps.Select(e => new CarouselProps()).ToArray());
            operatorCarousel.onChange += OnOperatorChange;
        }

        private void CreateClip()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new SpendMoneyRequest {Source = "clip", Amount = _clipCost});
        }

        private void HandleSpendMoneyResponse(SpendMoneyResponse resp)
        {
            if (resp.Source != "clip")
            {
                return;
            }

            if (!resp.OK)
            {
                noMoneyErr.Show(GetLocale("not_enough_money"));
                price.ShowNoMoney();
                return;
            }

            var track = trackCarousel.GetValue<TrackInfo>();
            track.HasClip = true;

            _clip.TrackId = track.Id;
            _clip.Name    = track.Name;

            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type    = WindowType.ProductionClipWork,
                Context = _clip
            });
        }

        private void OnDirectorChange(int index)
        {
            var director = staffData.Directors[index];
            directorSkill.text  = GetLocale("skill_value", director.Skill).ToUpper();
            directorPrice.text  = GetLocale("cost_value", director.Salary.GetMoney()).ToUpper();
            _clip.DirectorSkill = director.Skill;
            _directorPrice      = director.Salary;
            DisplayFullPrice();
        }

        private void OnOperatorChange(int index)
        {
            var clipOperator = staffData.Operators[index];
            operatorSkill.text  = GetLocale("skill_value", clipOperator.Skill).ToUpper();
            operatorPrice.text  = GetLocale("cost_value", clipOperator.Salary.GetMoney()).ToUpper();
            _clip.OperatorSkill = clipOperator.Skill;
            _operatorPrice      = clipOperator.Salary;
            DisplayFullPrice();
        }

        private void DisplayFullPrice()
        {
            var fullPrice = GetLocale("cost_value", _clipCost.GetMoney());
            price.SetValue(fullPrice.ToUpper());
        }

        protected override void BeforeShow(object ctx = null)
        {
            _clip = new ClipInfo {CreatorId = -1};

            CacheLastTracks();

            var anyTracks = _lastTracks.Any();
            var trackProps = anyTracks
                ? _lastTracks
                    .Select(e => new CarouselProps {Text = e.Name, Value = e})
                    .ToArray()
                : new[]
                {
                    new CarouselProps
                    {
                        Text  = GetLocale("no_tracks_yet").ToUpper(),
                        Value = new TrackInfo()
                    }
                };
            trackCarousel.Init(trackProps);
            startButton.interactable = anyTracks;

            OnDirectorChange(0);
            OnOperatorChange(0);
        }

        protected override void AfterShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.NewClipSelected);

            if (!HintsManager.Instance.ShowHint("tutorial_clip_page"))
            {
                MsgBroker.Instance
                    .Receive<SpendMoneyResponse>()
                    .Subscribe(HandleSpendMoneyResponse)
                    .AddTo(_disposable);
            }
        }

        protected override void AfterHide()
        {
            _clip = null;
            _lastTracks.Clear();
            _disposable.Clear();
        }

        private void CacheLastTracks()
        {
            var tracks = PlayerAPI.Data.History.TrackList
                .OrderByDescending(e => e.Id)
                .Where(e => !e.HasClip)
                .Take(TRACKS_CACHE);

            _lastTracks.AddRange(tracks);
        }
    }
}