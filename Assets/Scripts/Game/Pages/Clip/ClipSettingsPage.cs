using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Game.UI;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Game.Pages.Clip
{
    /// <summary>
    /// Страница настройки клипа
    /// </summary>
    public class ClipSettingsPage : Page
    {
        private const int TRACKS_CACHE = 10;
        
        [Header("Контролы")]
        [SerializeField] private Switcher trackSwitcher;
        [Space, SerializeField] private Switcher directorSwitcher;
        [SerializeField] private Text directorSkill;
        [SerializeField] private Text directorPrice;
        [Space, SerializeField] private Switcher operatorSwitcher;
        [SerializeField] private Text operatorSkill;
        [SerializeField] private Text operatorPrice;
        [Space, SerializeField] private Price price;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")]
        [SerializeField] private ClipWorkingPage workingPage;

        [Header("Данные")]
        [SerializeField] private ClipStaffData data;

        private ClipInfo _clip;
        private List<TrackInfo> _lastTracks = new List<TrackInfo>(TRACKS_CACHE);
        private int _directorPrice;
        private int _operatorPrice;

        private int _fullPrice => _directorPrice + _operatorPrice;
        
        private void Start()
        {
            startButton.onClick.AddListener(CreateClip);
            
            // todo: Localize
            directorSwitcher.InstantiateElements(data.Directors.Select(e => e.NameKey));
            operatorSwitcher.InstantiateElements(data.Operators.Select(e => e.NameKey));
            
            directorSwitcher.onIndexChange += OnDirectorChange;
            operatorSwitcher.onIndexChange += OnOperatorChange;
        }

        /// <summary>
        /// Запускает создание клипа
        /// </summary>
        private void CreateClip()
        {
            if (!PlayerManager.Instance.SpendMoney(_fullPrice))
            {
                price.ShowNoMoney();
                return;
            }
            
            var track = _lastTracks[trackSwitcher.ActiveIndex];
            track.HasClip = true;
            
            _clip.TrackId = track.Id;
            workingPage.CreateClip(_clip);
            Close();
        }

        /// <summary>
        /// Обработчик смены режисера 
        /// </summary>
        private void OnDirectorChange(int index)
        {
            var director = data.Directors[index];
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
            var clipOperator = data.Operators[index];
            operatorSkill.text = $"Навык: {clipOperator.Skill}";
            operatorPrice.text = $"Стоимость: {clipOperator.Salary}$";
            _clip.OperatorSkill = clipOperator.Skill;
            _operatorPrice = clipOperator.Salary;
            DisplayFullPrice();
        }

        /// <summary>
        /// Отображает полную стоимость клипа
        /// </summary>
        private void DisplayFullPrice() => price.SetValue($"СТОИМОСТЬ: {_fullPrice}$");

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

            if (_lastTracks.Any())
            {
                trackSwitcher.InstantiateElements(_lastTracks.Select(e => e.Name));
                startButton.interactable = true;
            }
            else
            {
                trackSwitcher.InstantiateElements(new[] {"Нет треков"});
                startButton.interactable = false;
            }
            
            OnDirectorChange(0);
            OnOperatorChange(0);
        }

        protected override void AfterPageClose()
        {
            _clip = null;
            _lastTracks.Clear();
            
            directorSwitcher.ResetActive(true);
            operatorSwitcher.ResetActive(true);
        }

        #endregion

        private void OnDestroy()
        {
            directorSwitcher.onIndexChange -= OnDirectorChange;
            operatorSwitcher.onIndexChange -= OnOperatorChange;
        }
    }
    
    [Serializable]
    public class ClipStaff
    {
        public string Name;
        public int Price;
        public int Skill;
    }
}