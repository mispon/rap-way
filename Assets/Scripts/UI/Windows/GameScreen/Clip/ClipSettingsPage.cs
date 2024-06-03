using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Extensions;
// using Firebase.Analytics;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using MessageBroker.Messages.UI;
using Models.Production;
using ScriptableObjects;
using Sirenix.OdinInspector;
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
        [BoxGroup("Controls"), SerializeField] private Carousel trackCarousel;
        [BoxGroup("Controls"), SerializeField] private Carousel directorCarousel;
        [BoxGroup("Controls"), SerializeField] private Carousel operatorCarousel;
        [BoxGroup("Controls"), SerializeField] private Button startButton;
        
        [BoxGroup("Labels"), SerializeField] private Text directorSkill;
        [BoxGroup("Labels"), SerializeField] private Text directorPrice;
        [BoxGroup("Labels"), SerializeField] private Text operatorSkill;
        [BoxGroup("Labels"), SerializeField] private Text operatorPrice;
        
        [BoxGroup("Price"), SerializeField] private GameError noMoneyErr;
        [BoxGroup("Price"), SerializeField] private Price price;

        [BoxGroup("Data"), SerializeField] private ClipStaffData staffData;

        private ClipInfo _clip;
        private int _directorPrice;
        private int _operatorPrice;
        
        private const int TRACKS_CACHE = 10;
        private readonly List<TrackInfo> _lastTracks = new(TRACKS_CACHE);

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
            MsgBroker.Instance.Publish(new SpendMoneyRequest {Amount = _clipCost});
        }

        private void HandleSpendMoneyResponse(SpendMoneyResponse resp)
        {
            if (!resp.OK)
            {
                noMoneyErr.Show(GetLocale("not_enough_money"));
                price.ShowNoMoney();
                return;
            }
            
            var track = trackCarousel.GetValue<TrackInfo>();
            track.HasClip = true;

            _clip.TrackId = track.Id;
            _clip.Name = track.Name;
            
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionClipWork,
                Context = _clip
            });
        }

        private void OnDirectorChange(int index)
        {
            var director = staffData.Directors[index];
            directorSkill.text = GetLocale("skill_value", director.Skill).ToUpper();
            directorPrice.text = GetLocale("cost_value", director.Salary.GetMoney()).ToUpper();
            _clip.DirectorSkill = director.Skill;
            _directorPrice = director.Salary;
            DisplayFullPrice();
        }

        private void OnOperatorChange(int index)
        {
            var clipOperator = staffData.Operators[index];
            operatorSkill.text = GetLocale("skill_value", clipOperator.Skill).ToUpper();
            operatorPrice.text = GetLocale("cost_value", clipOperator.Salary.GetMoney()).ToUpper();
            _clip.OperatorSkill = clipOperator.Skill;
            _operatorPrice = clipOperator.Salary;
            DisplayFullPrice();
        }
        
        private void DisplayFullPrice()
        {
            string fullPrice = GetLocale("cost_value", _clipCost.GetMoney());
            price.SetValue(fullPrice.ToUpper());
        }
        
        protected override void BeforeShow(object ctx = null)
        {
            _clip = new ClipInfo();

            CacheLastTracks();

            bool anyTracks = _lastTracks.Any();
            var trackProps = anyTracks
                ? _lastTracks
                    .Select(e => new CarouselProps {Text = e.Name, Value = e})
                    .ToArray()
                : new[] {
                    new CarouselProps
                    {
                        Text = GetLocale("no_tracks_yet").ToUpper(),
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
            HintsManager.Instance.ShowHint("tutorial_clip_page");
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewClipSelected);

            MsgBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(HandleSpendMoneyResponse)
                .AddTo(_disposable);
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