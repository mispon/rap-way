using System.Collections.Generic;
using System.Linq;
using Core;
using Data;
using Game.UI;
using Game.UI.GameScreen;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница настроек концерта
    /// </summary>
    public class ConcertSettingsPage : Page
    {
        private const int ALBUMS_CACHE = 5;
        
        [Header("Компоненты")]
        [SerializeField] private Switcher placeSwitcher;
        [SerializeField] private Text placeCapacityLabel;
        [SerializeField] private Text fansRequirementLabel;
        [SerializeField] private Switcher albumsSwitcher;
        [Space, SerializeField] private Slider ticketCostSlider;
        [SerializeField] private Text ticketCost;
        [Space, SerializeField] private Price concertPrice;
        [SerializeField] private Button startButton;
        
        [Header("Страница разработки")]
        [SerializeField] private ConcertWorkingPage workingPage;

        [Header("Данные")]
        [SerializeField] private ConcertPlacesData data;

        private ConcertInfo _concert;
        private int _placeCost;
        private readonly List<AlbumInfo> _lastAlbums = new List<AlbumInfo>(ALBUMS_CACHE);

        private void Start()
        {
            startButton.onClick.AddListener(CreateConcert);
            
            placeSwitcher.InstantiateElements(data.Places.Select(e => e.NameKey));
            
            placeSwitcher.onIndexChange += OnPlaceChanged;
            albumsSwitcher.onIndexChange += OnAlbumsChanged;
            ticketCostSlider.onValueChanged.AddListener(OnTicketPriceChanged);

            ResetTicketCost();
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

            _concert.Id = PlayerManager.GetNextProductionId<ConcertInfo>();
            
            workingPage.StartWork(_concert);
            Close();
        }
        
        /// <summary>
        /// Обработчик изменения площадки 
        /// </summary>
        private void OnPlaceChanged(int index)
        {
            var place = data.Places[index];

            _concert.LocationId = index;
            _concert.LocationName = place.NameKey;
            _concert.LocationCapacity = place.Capacity;
            placeCapacityLabel.text = $"ВМЕСТИТЕЛЬНОСТЬ: {place.Capacity}";
            
            _placeCost = place.Cost;
            concertPrice.SetValue($"АРЕНДА: {_placeCost} $");

            ticketCostSlider.minValue = place.TicketMinCost;
            ticketCostSlider.maxValue = place.TicketMaxCost;
            ResetTicketCost();
            
            fansRequirementLabel.text = $"НЕОБХОДИМО ФАНАТОВ: {place.FansRequirement}";
            CheckConcertConditions(place.FansRequirement);
        }
        
        /// <summary>
        /// Обработчик изменения альбома 
        /// </summary>
        private void OnAlbumsChanged(int index)
        {
            var album = _lastAlbums[index];
            _concert.AlbumId = album.Id;
        }
        
        /// <summary>
        /// Обработчик изменения цены билета 
        /// </summary>
        private void OnTicketPriceChanged(float value)
        {
            var cost = (int) value;
            _concert.TicketCost = cost;
            ticketCost.text = $"{cost} $";
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
            ticketCost.text = $"{minValue} $";
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
        
        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            _concert = new ConcertInfo();
            CacheLastAlbums();

            albumsSwitcher.InstantiateElements(
                _lastAlbums.Any() ? _lastAlbums.Select(e => e.Name) : new[] {"Нет альбомов"}
            );

            OnPlaceChanged(0);
            
            GameScreenController.Instance.HideProductionGroup();
        }

        protected override void AfterPageClose()
        {
            _concert = null;
            _lastAlbums.Clear();
            
            placeSwitcher.ResetActive(true);
            albumsSwitcher.ResetActive(true);
            ResetTicketCost();
        }

        #endregion

        private void OnDestroy()
        {
            placeSwitcher.onIndexChange -= OnPlaceChanged;
            albumsSwitcher.onIndexChange -= OnAlbumsChanged;
            ticketCostSlider.onValueChanged.RemoveAllListeners();
        }
    }
}