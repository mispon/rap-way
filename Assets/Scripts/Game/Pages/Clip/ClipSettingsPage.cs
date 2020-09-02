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
using Utils.Carousel;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница настройки клипа
    /// </summary>
    public class ClipSettingsPage : Page
    {
        private const int TRACKS_CACHE = 10;

        [Header("Контролы")] 
        [SerializeField] private Carousel trackCarousel;
        [Space, SerializeField] private Carousel directorCarousel;
        [SerializeField] private Text directorSkill;
        [SerializeField] private Text directorPrice;
        [Space, SerializeField] private Carousel operatorCarousel;
        [SerializeField] private Text operatorSkill;
        [SerializeField] private Text operatorPrice;
        [Space, SerializeField] private Price price;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")] [SerializeField]
        private ClipWorkingPage workingPage;

        [Header("Данные")] 
        [SerializeField] private ClipStaffData staffData;
        [SerializeField] private ImagesBank imagesBank;

        private ClipInfo _clip;
        private int _directorPrice;
        private int _operatorPrice;
        private readonly List<TrackInfo> _lastTracks = new List<TrackInfo>(TRACKS_CACHE);

        private int FullPrice => _directorPrice + _operatorPrice;

        private void Start()
        {
            startButton.onClick.AddListener(CreateClip);

            SetupStaffCarousels();
        }

        /// <summary>
        /// Инициализирует карусели 
        /// </summary>
        private void SetupStaffCarousels()
        {
            var directorProps = ConvertStaffToCarouselProps(staffData.Directors, imagesBank.Directors);
            directorCarousel.Init(directorProps);
            directorCarousel.onChange += OnDirectorChange;
            
            var operatorProps = ConvertStaffToCarouselProps(staffData.Operators, imagesBank.Operators);
            operatorCarousel.Init(operatorProps);
            operatorCarousel.onChange += OnOperatorChange;
        }
        
        /// <summary>
        /// Конвертирует данные персонала в свойства карусели 
        /// </summary>
        private CarouselProps[] ConvertStaffToCarouselProps(IEnumerable<ClipStaff> staffArray, IReadOnlyList<Sprite> spriteArray)
        {
            return staffArray.Select((clipStaffInfo, index) => new CarouselProps
            {
                Text = clipStaffInfo.NameKey,
                Sprite = spriteArray[index],
                Value = index
            }).ToArray();
        }

        /// <summary>
        /// Конвертирует трек в свойство карусели
        /// </summary>
        private CarouselProps ConvertTrackToCarouselProps(TrackInfo trackInfo)
        {
            return new CarouselProps
            {
                Text = trackInfo.Name,
                Value = trackInfo
            };
        }
        
        /// <summary>
        /// Запускает создание клипа
        /// </summary>
        private void CreateClip()
        {
            SoundManager.Instance.PlayClick();

            if (!PlayerManager.Instance.SpendMoney(FullPrice))
            {
                price.ShowNoMoney();
                return;
            }

            var track = trackCarousel.GetValue<TrackInfo>();
            track.HasClip = true;

            _clip.TrackId = track.Id;
            _clip.Name = track.Name;

            var directorImage = imagesBank.Directors[directorCarousel.GetValue<int>()];
            var operatorImage = imagesBank.Operators[operatorCarousel.GetValue<int>()];
            workingPage.StartWork(_clip, directorImage, operatorImage);
            Close();
        }

        /// <summary>
        /// Обработчик смены режисера 
        /// </summary>
        private void OnDirectorChange(int index)
        {
            var director = staffData.Directors[index];
            directorSkill.text = $"Навык: {director.Skill}";
            directorPrice.text = $"Стоимость: {director.Salary}$";
            _clip.DirectorSkill = director.Skill;
            _directorPrice = director.Salary;
            DisplayFullPrice();
        }

        /// <summary>
        /// Обработчик смены оператора 
        /// </summary>
        private void OnOperatorChange(int index)
        {
            var clipOperator = staffData.Operators[index];
            operatorSkill.text = $"Навык: {clipOperator.Skill}";
            operatorPrice.text = $"Стоимость: {clipOperator.Salary}$";
            _clip.OperatorSkill = clipOperator.Skill;
            _operatorPrice = clipOperator.Salary;
            DisplayFullPrice();
        }

        /// <summary>
        /// Отображает полную стоимость клипа
        /// </summary>
        private void DisplayFullPrice() => price.SetValue($"СТОИМОСТЬ: {FullPrice}$");

        /// <summary>
        /// Кэширует самые новые треки игрока на которые еще не снимался клип
        /// </summary>
        private void CacheLastTracks()
        {
            var tracks = PlayerManager.Data.History.TrackList
                .OrderByDescending(e => e.Id)
                .Where(e => !e.HasClip)
                .Take(TRACKS_CACHE);
            _lastTracks.AddRange(tracks);
        }

        #region PAGE EVENTS

        protected override void BeforePageOpen()
        {
            _clip = new ClipInfo();

            CacheLastTracks();

            var anyTracks = _lastTracks.Any();
            var trackProps = anyTracks 
                ? _lastTracks.Select(ConvertTrackToCarouselProps).ToArray() 
                : new[] { new CarouselProps { Text = "Нет треков", Value = new TrackInfo() } };
            trackCarousel.Init(trackProps);
            startButton.interactable = anyTracks;

            OnDirectorChange(0);
            OnOperatorChange(0);

            GameScreenController.Instance.HideProductionGroup();
        }

        protected override void AfterPageClose()
        {
            _clip = null;
            _lastTracks.Clear();
        }

        #endregion
    }
}