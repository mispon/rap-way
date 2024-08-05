using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using Extensions;
using Firebase.Analytics;
using Game;
using Game.Player.Team;
using Game.Production;
using MessageBroker;
using MessageBroker.Messages.Player;
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

namespace UI.Windows.GameScreen.Concert
{
    public class ConcertSettingsPage : Page
    {
        [Header("Controls")]
        [SerializeField] private Carousel placeCarousel;
        [SerializeField] private Carousel albumsCarousel;
        [SerializeField] private Slider ticketCostSlider;
        [SerializeField] private Button startButton;

        [Header("Labels")]
        [SerializeField] private Text placeCapacityLabel;
        [SerializeField] private Text fansRequirementLabel;
        [SerializeField] private Text ticketCost;
        [SerializeField] private GameObject cooldownIcon;

        [Header("Avatars")]
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Image prAvatar;

        [Header("Price")]
        [SerializeField] private Price concertPrice;
        [SerializeField] private GameError noMoneyErr;

        [Header("Data")]
        [SerializeField] private ConcertPlacesData placeData;
        [SerializeField] private ImagesBank imagesBank;

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
            MsgBroker.Instance.Publish(new SpendMoneyRequest { Amount = _placeCost });
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

            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.ProductionConcertWork,
                Context = _concert
            });
        }

        private void SetupPlaceCarousel()
        {
            var placeProps = placeData.Places
                .Select(e => new CarouselProps { Text = e.NameKey, Value = e })
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
            bool canStart = PlayerAPI.Data.Fans >= fansRequirement;
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

        protected override void BeforeShow(object ctx = null)
        {
            _concert = new ConcertInfo();
            CacheLastAlbums();

            var anyAlbums = _lastAlbums.Any();
            var albumProps = anyAlbums
                ? _lastAlbums
                    .Select(e => new CarouselProps { Text = e.Name, Value = e })
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

            MsgBroker.Instance
                .Receive<TeamSalaryMessage>()
                .Subscribe(e => ResetTeam())
                .AddTo(_disposable);
        }

        protected override void AfterShow(object ctx = null)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.NewConcertSelected);
            HintsManager.Instance.ShowHint("tutorial_concert_page");

            MsgBroker.Instance
                .Receive<SpendMoneyResponse>()
                .Subscribe(HandleSpendMoneyResponse)
                .AddTo(_disposable);
        }

        protected override void AfterHide()
        {
            _concert = null;
            _lastAlbums.Clear();
            ResetTicketCost();

            _disposable.Clear();
        }

        private void CacheLastAlbums()
        {
            var albums = PlayerAPI.Data.History.AlbumList
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

        private void ResetTeam()
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