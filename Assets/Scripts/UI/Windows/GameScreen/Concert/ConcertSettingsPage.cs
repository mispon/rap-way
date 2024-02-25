using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Events;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game;
using Game.Player;
using Game.Player.Team;
using Game.Production;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using Models.Production;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UI.Controls.Carousel;
using UI.Controls.Error;
using UI.Controls.Money;
using UI.GameScreen;
using UI.Windows.GameScreen;
using UI.Windows.Tutorial;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using EventType = Core.Events.EventType;

namespace UI.Windows.Pages.Concert
{
    /// <summary>
    /// Страница настроек концерта
    /// </summary>
    public class ConcertSettingsPage : Page
    {
        [BoxGroup("Controls"), SerializeField] private Carousel placeCarousel;
        [BoxGroup("Controls"), SerializeField] private Carousel albumsCarousel;
        [BoxGroup("Controls"), SerializeField] private Slider ticketCostSlider;
        [BoxGroup("Controls"), SerializeField] private Button startButton;
        
        [BoxGroup("Labels"), SerializeField] private Text placeCapacityLabel;
        [BoxGroup("Labels"), SerializeField] private Text fansRequirementLabel;
        [BoxGroup("Labels"), SerializeField] private Text ticketCost;
        [BoxGroup("Labels"), SerializeField] private GameObject cooldownIcon;
        
        [BoxGroup("Avatars"), SerializeField] private Image managerAvatar;
        [BoxGroup("Avatars"), SerializeField] private Image prAvatar;
        
        [BoxGroup("Price"), SerializeField] private Price concertPrice;
        [BoxGroup("Price"), SerializeField] private GameError noMoneyErr;
        
        [BoxGroup("Pages"), SerializeField] private ConcertWorkingPage workingPage;
        [BoxGroup("Pages"), SerializeField] private Page productSelectionPage;
        
        [BoxGroup("Data"), SerializeField] private ConcertPlacesData placeData;
        [BoxGroup("Data"), SerializeField] private ImagesBank imagesBank;
        
        private ConcertInfo _concert;
        private int _placeCost;
        
        private const int MAX_ALBUMS_COUNT = 5;
        private readonly List<AlbumInfo> _lastAlbums = new(MAX_ALBUMS_COUNT);

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            startButton.onClick.AddListener(CreateConcert);
            ticketCostSlider.onValueChanged.AddListener(OnTicketPriceChanged);

            SetupPlaceCarousel();
        }
        
        private void CreateConcert()
        {
            SoundManager.Instance.PlaySound(UIActionType.Click);
            MsgBroker.Instance.Publish(new SpendMoneyRequest {Amount = _placeCost});
        }

        private void HandleSpendMoneyResponse(SpendMoneyResponse resp)
        {
            if (!resp.OK)
            {
                noMoneyErr.Show(GetLocale("not_enough_money"));
                concertPrice.ShowNoMoney();
                return;
            }
            
            var album = albumsCarousel.GetValue<AlbumInfo>();
            album.ConcertAmounts += 1;

            _concert.AlbumId = album.Id;
            _concert.Id = ProductionManager.GetNextProductionId<ConcertInfo>();
            _concert.TicketCost = Mathf.RoundToInt(ticketCostSlider.value);

            productSelectionPage.Close();
            workingPage.StartWork(_concert);
            
            Close();
        }
        
        private void SetupPlaceCarousel()
        {
            var placeProps = placeData.Places
                .Select(e => new CarouselProps {Text = e.NameKey, Value = e})
                .ToArray();
            
            placeCarousel.Init(placeProps);
            placeCarousel.onChange += OnPlaceChanged;
        }
        
        private void OnPlaceChanged(int index)
        {
            var place = placeData.Places[index];

            _concert.LocationId = index;
            _concert.LocationName = place.NameKey;
            _concert.LocationCapacity = place.Capacity;
            _concert.MaxTicketCost = place.TicketMaxCost;
            placeCapacityLabel.text = GetLocale("concert_capacity", place.Capacity.GetDisplay()).ToUpper();

            _placeCost = place.Cost;
            concertPrice.SetValue(GetLocale("concert_rent", _placeCost.GetMoney()).ToUpper());

            ticketCostSlider.minValue = place.TicketMinCost;
            ticketCostSlider.maxValue = place.TicketMaxCost;
            ResetTicketCost();

            fansRequirementLabel.text = GetLocale("concert_fans_requirement", place.FansRequirement.GetDisplay());
            CheckConcertConditions(place.FansRequirement);
        }
        
        private void CheckConcertConditions(int fansRequirement)
        {
            bool canStart = PlayerManager.Data.Fans >= fansRequirement;
            canStart &= _lastAlbums.Any();
            
            bool hasCooldown = GameManager.Instance.GameStats.ConcertCooldown > 0;
            cooldownIcon.SetActive(hasCooldown);
            
            canStart &= !hasCooldown;

            startButton.interactable = canStart;
        }
        
        private void OnTicketPriceChanged(float value)
        {
            int cost = Mathf.RoundToInt(value);
            _concert.TicketCost = cost;
            ticketCost.text = $"{cost.GetMoney()}";
        }
        
        protected override void BeforePageOpen()
        {
            _concert = new ConcertInfo();
            CacheLastAlbums();

            var anyAlbums = _lastAlbums.Any();
            var albumProps = anyAlbums
                ? _lastAlbums
                    .Select(e => new CarouselProps {Text = e.Name, Value = e})
                    .ToArray()
                : new[] {
                    new CarouselProps
                    {
                        Text = GetLocale("no_albums_yet"),
                        Value = new AlbumInfo()
                    }
                };
            albumsCarousel.Init(albumProps);

            SetupTeam();
            OnPlaceChanged(0);

            EventManager.AddHandler(EventType.UncleSamsParty, ResetTeam);
            GameScreenController.Instance.HideProductionGroup();
        }
        
        protected override void AfterPageOpen()
        {
            HintsManager.Instance.ShowHint("tutorial_concert_page");
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewConcertSelected);

            MsgBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(HandleSpendMoneyResponse)
                .AddTo(_disposable);
        }

        protected override void AfterPageClose()
        {
            EventManager.RemoveHandler(EventType.UncleSamsParty, ResetTeam);
            
            _concert = null;
            _lastAlbums.Clear();
            ResetTicketCost();
            
            _disposable.Clear();
        }
        
        private void CacheLastAlbums()
        {
            var albums = PlayerManager.Data.History.AlbumList
                .Where(e => e.ConcertAmounts < 3)
                .OrderByDescending(e => e.Id)
                .Take(MAX_ALBUMS_COUNT);
            
            _lastAlbums.AddRange(albums);
        }
        
        private void SetupTeam()
        {
            managerAvatar.sprite = TeamManager.IsAvailable(TeammateType.Manager)
                ? imagesBank.ProducerActive
                : imagesBank.ProducerInactive;
            
            prAvatar.sprite = TeamManager.IsAvailable(TeammateType.PrMan)
                ? imagesBank.PrManActive
                : imagesBank.PrManInactive;
        }
        
        private void ResetTeam(object[] args)
        {
            managerAvatar.sprite = imagesBank.ProducerInactive;
            prAvatar.sprite = imagesBank.PrManInactive;
        }
        
        private void ResetTicketCost()
        {
            int minValue = Mathf.RoundToInt(ticketCostSlider.minValue);
            ticketCostSlider.SetValueWithoutNotify(minValue);
            ticketCost.text = $"{minValue.GetMoney()}";
        }
    }
}