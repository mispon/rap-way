using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.UI;
using Game.UI.GameScreen;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils.Carousel;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница настроек концерта
    /// </summary>
    public class ConcertSettingsPage : Page
    {
        private const int ALBUMS_CACHE = 5;

        [Header("Компоненты")] 
        [SerializeField] private Carousel placeCarousel;
        [SerializeField] private Text placeCapacityLabel;
        [SerializeField] private Text fansRequirementLabel;
        [SerializeField] private Carousel albumsCarousel;
        
        [Space, SerializeField] private Slider ticketCostSlider;
        [SerializeField] private Text ticketCost;
        
        [Space, SerializeField] private Price concertPrice;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")] 
        [SerializeField] private ConcertWorkingPage workingPage;

        [Header("Данные")] 
        [SerializeField] private ConcertPlacesData placeData;

        private ConcertInfo _concert;
        private int _placeCost;
        private readonly List<AlbumInfo> _lastAlbums = new List<AlbumInfo>(ALBUMS_CACHE);

        private void Start()
        {
            startButton.onClick.AddListener(CreateConcert);
            ticketCostSlider.onValueChanged.AddListener(OnTicketPriceChanged);

            SetupPlaceCarousel();
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
            _concert.AlbumId = album.Id;
            _concert.Id = PlayerManager.GetNextProductionId<ConcertInfo>();

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
            placeCapacityLabel.text = $"ВМЕСТИТЕЛЬНОСТЬ: {place.Capacity:N0}";

            _placeCost = place.Cost;
            concertPrice.SetValue($"АРЕНДА: {_placeCost:N0}$");

            ticketCostSlider.minValue = place.TicketMinCost;
            ticketCostSlider.maxValue = place.TicketMaxCost;
            ResetTicketCost();

            fansRequirementLabel.text = $"*НЕОБХОДИМО {place.FansRequirement:N0} ФАНАТОВ";
            CheckConcertConditions(place.FansRequirement);
        }

        /// <summary>
        /// Обработчик изменения цены билета 
        /// </summary>
        private void OnTicketPriceChanged(float value)
        {
            var cost = (int) value;
            _concert.TicketCost = cost;
            ticketCost.text = $"{cost:N0}$";
        }

        /// <summary>
        /// Кэширует самые новые альбомы игрока
        /// </summary>
        private void CacheLastAlbums()
        {
            var albums = PlayerManager.Data.History.AlbumList
                .OrderByDescending(e => e.Id)
                .Take(ALBUMS_CACHE);
            _lastAlbums.AddRange(albums);
        }

        /// <summary>
        /// Сбрасывает стоимость билетов в минимальное значение
        /// </summary>
        private void ResetTicketCost()
        {
            var minValue = ticketCostSlider.minValue;
            ticketCostSlider.SetValueWithoutNotify(minValue);
            ticketCost.text = $"{minValue:N0}$";
        }

        /// <summary>
        /// Проверяет соответствие всех требований 
        /// </summary>
        private void CheckConcertConditions(int fansRequirement)
        {
            bool canStart = PlayerManager.Data.Fans >= fansRequirement;
            canStart &= _lastAlbums.Any();

            startButton.interactable = canStart;
        }

        protected override void BeforePageOpen()
        {
            _concert = new ConcertInfo();
            CacheLastAlbums();

            var anyAlbums = _lastAlbums.Any();
            var albumProps = anyAlbums
                ? _lastAlbums.Select(ConvertAlbumToCarouselProps).ToArray()
                : new[] {new CarouselProps {Text = "Нет альбомов", Value = new AlbumInfo()}};
            albumsCarousel.Init(albumProps);

            OnPlaceChanged(0);

            GameScreenController.Instance.HideProductionGroup();
        }

        protected override void AfterPageClose()
        {
            _concert = null;
            _lastAlbums.Clear();
            ResetTicketCost();
        }
    }
}