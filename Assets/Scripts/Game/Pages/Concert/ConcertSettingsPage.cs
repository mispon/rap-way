using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Enums;
using Game.UI;
using Game.UI.GameScreen;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;
using Utils.Extensions;
using EventType = Core.EventType;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница настроек концерта
    /// </summary>
    public class ConcertSettingsPage : Page
    {
        private const int MAX_ALBUMS_COUNT = 5;

        [Header("Контролы")]
        [SerializeField] private Carousel placeCarousel;
        [SerializeField] private Carousel albumsCarousel;
        [SerializeField] private Slider ticketCostSlider;
        [SerializeField] private Button startButton;
        [SerializeField] private GameObject cooldownIcon;
        
        [Header("Компоненты")]
        [SerializeField] private Text placeCapacityLabel;
        [SerializeField] private Text fansRequirementLabel;
        [SerializeField] private Text ticketCost;
        [SerializeField] private Price concertPrice;
        [Space]
        [SerializeField] private Image managerAvatar;
        [SerializeField] private Image prAvatar;

        [Header("Страница разработки")] 
        [SerializeField] private ConcertWorkingPage workingPage;

        [Header("Страница выбора")] 
        [SerializeField]
        private Page productSelectionPage;
        
        [Header("Данные")] 
        [SerializeField] private ConcertPlacesData placeData;
        [SerializeField] private ImagesBank imagesBank;

        private ConcertInfo _concert;
        private int _placeCost;
        private readonly List<AlbumInfo> _lastAlbums = new(MAX_ALBUMS_COUNT);

        private void Start()
        {
            startButton.onClick.AddListener(CreateConcert);
            ticketCostSlider.onValueChanged.AddListener(OnTicketPriceChanged);

            SetupPlaceCarousel();
        }
        
        protected override void AfterPageOpen()
        {
            TutorialManager.Instance.ShowTutorial("tutorial_concert_page");
        }

        /// <summary>
        /// Инициализирует карусель концертов 
        /// </summary>
        private void SetupPlaceCarousel()
        {
            var placeProps = placeData.Places.Select(ConvertPlaceToCarouselProps).ToArray();
            placeCarousel.Init(placeProps);
            placeCarousel.onChange += OnPlaceChanged;
        }
        
        /// <summary>
        /// Отображает состояние членов команды
        /// </summary>
        private void SetupTeam()
        {
            managerAvatar.sprite = TeamManager.IsAvailable(TeammateType.Manager)
                ? imagesBank.ProducerActive
                : imagesBank.ProducerInactive;
            prAvatar.sprite = TeamManager.IsAvailable(TeammateType.PrMan)
                ? imagesBank.PrManActive
                : imagesBank.PrManInactive;
        }

        /// <summary>
        /// Конвертирует площадку в свойство карусели
        /// </summary>
        private static CarouselProps ConvertPlaceToCarouselProps(ConcertPlace placeInfo)
        {
            return new CarouselProps
            {
                Text = placeInfo.NameKey,
                Value = placeInfo
            };
        }

        /// <summary>
        /// Конвертирует альбом в свойство карусели
        /// </summary>
        private static CarouselProps ConvertAlbumToCarouselProps(AlbumInfo albumInfo)
        {
            return new CarouselProps
            {
                Text = albumInfo.Name,
                Value = albumInfo
            };
        }

        /// <summary>
        /// Запускает подготовку концерта 
        /// </summary>
        private void CreateConcert()
        {
            SoundManager.Instance.PlayClick();

            if (!PlayerManager.Instance.SpendMoney(_placeCost))
            {
                concertPrice.ShowNoMoney();
                return;
            }

            var album = albumsCarousel.GetValue<AlbumInfo>();
            album.ConcertAmounts += 1;

            _concert.AlbumId = album.Id;
            _concert.Id = PlayerManager.GetNextProductionId<ConcertInfo>();
            _concert.TicketCost = Mathf.RoundToInt(ticketCostSlider.value);

            productSelectionPage.Close();
            workingPage.StartWork(_concert);
            Close();
        }

        /// <summary>
        /// Обработчик изменения площадки 
        /// </summary>
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

        /// <summary>
        /// Обработчик изменения цены билета 
        /// </summary>
        private void OnTicketPriceChanged(float value)
        {
            int cost = Mathf.RoundToInt(value);
            _concert.TicketCost = cost;
            ticketCost.text = $"{cost.GetMoney()}";
        }

        /// <summary>
        /// Кэширует самые новые альбомы игрока
        /// </summary>
        private void CacheLastAlbums()
        {
            var albums = PlayerManager.Data.History.AlbumList
                .Where(e => e.ConcertAmounts < 3)
                .OrderByDescending(e => e.Id)
                .Take(MAX_ALBUMS_COUNT);
            _lastAlbums.AddRange(albums);
        }

        /// <summary>
        /// Сбрасывает стоимость билетов в минимальное значение
        /// </summary>
        private void ResetTicketCost()
        {
            int minValue = Mathf.RoundToInt(ticketCostSlider.minValue);
            ticketCostSlider.SetValueWithoutNotify(minValue);
            ticketCost.text = $"{minValue.GetMoney()}";
        }

        /// <summary>
        /// Сбрасывает состояние команды
        /// </summary>
        private void ResetTeam(object[] args)
        {
            managerAvatar.sprite = imagesBank.ProducerInactive;
            prAvatar.sprite = imagesBank.PrManInactive;
        }

        /// <summary>
        /// Проверяет соответствие всех требований 
        /// </summary>
        private void CheckConcertConditions(int fansRequirement)
        {
            bool canStart = PlayerManager.Data.Fans >= fansRequirement;
            canStart &= _lastAlbums.Any();
            
            bool hasCooldown = GameManager.Instance.GameStats.ConcertCooldown > 0;
            cooldownIcon.SetActive(hasCooldown);
            
            canStart &= !hasCooldown;

            startButton.interactable = canStart;
        }

        protected override void BeforePageOpen()
        {
            _concert = new ConcertInfo();
            CacheLastAlbums();

            var anyAlbums = _lastAlbums.Any();
            var albumProps = anyAlbums
                ? _lastAlbums.Select(ConvertAlbumToCarouselProps).ToArray()
                : new[]
                {
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

        protected override void AfterPageClose()
        {
            EventManager.RemoveHandler(EventType.UncleSamsParty, ResetTeam);
            
            _concert = null;
            _lastAlbums.Clear();
            ResetTicketCost();
        }
    }
}