using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Firebase.Analytics;
using Game.Pages.ProductSelection;
using Game.UI;
using Game.UI.GameError;
using Game.UI.GameScreen;
using MessageBroker.Messages.State;
using Models.Info.Production;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;
using Utils.Extensions;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница настройки клипа
    /// </summary>
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
        
        [BoxGroup("Pages"), SerializeField] private ClipWorkingPage workingPage;
        [BoxGroup("Pages"), SerializeField] private ProductSelectionPage productSelectionPage;
        
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
            SendMessage(new SpendMoneyRequest {Amount = _clipCost});
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
            
            productSelectionPage.Close();
            workingPage.StartWork(_clip);
            
            Close();
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
        
        protected override void BeforePageOpen()
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

            GameScreenController.Instance.HideProductionGroup();
        }
        
        protected override void AfterPageOpen()
        {
            TutorialManager.Instance.ShowTutorial("tutorial_clip_page");
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewClipSelected);
            
            RecvMessage<SpendMoneyResponse>(HandleSpendMoneyResponse, _disposable);
        }

        protected override void AfterPageClose()
        {
            _clip = null;
            _lastTracks.Clear();
            _disposable.Clear();
        }
        
        private void CacheLastTracks()
        {
            var tracks = PlayerManager.Data.History.TrackList
                .OrderByDescending(e => e.Id)
                .Where(e => !e.HasClip)
                .Take(TRACKS_CACHE);
            
            _lastTracks.AddRange(tracks);
        }
    }
}