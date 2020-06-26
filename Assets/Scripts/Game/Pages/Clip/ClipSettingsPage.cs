using System;
using System.Collections.Generic;
using System.Linq;
using Models.Production;
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
        [Space, SerializeField] private Text fullPrice;
        [SerializeField] private Button startButton;

        [Header("Страница разработки")]
        [SerializeField] private ClipWorkingPage workingPage;

        [Header("Данные")]
        [SerializeField] private ClipStaff[] directorsList;
        [SerializeField] private ClipStaff[] operatorsList;

        private ClipInfo _clip;
        private List<TrackInfo> _lastTracks = new List<TrackInfo>(TRACKS_CACHE);
        private int _directorPrice;
        private int _operatorPrice;

        private int _fullPrice => _directorPrice + _operatorPrice;
        
        private void Start()
        {
            startButton.onClick.AddListener(CreateClip);
            directorSwitcher.onIndexChange += OnDirectorChange;
            operatorSwitcher.onIndexChange += OnOperatorChange;
        }

        private void CreateClip()
        {
            // todo: check money and pay team
            
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
            var director = directorsList[index];
            directorSkill.text = $"Навык: {director.Skill}";
            directorPrice.text = $"Стоимость: {director.Price}";
            _directorPrice = director.Price;
            DisplayFullPrice();
        }
        
        /// <summary>
        /// Обработчик смены оператора 
        /// </summary>
        private void OnOperatorChange(int index)
        {
            var clipOperator = operatorsList[index];
            operatorSkill.text = $"Навык: {clipOperator.Skill}";
            operatorPrice.text = $"Стоимость: {clipOperator.Price}";
            _operatorPrice = clipOperator.Price;
            DisplayFullPrice();
        }

        /// <summary>
        /// Отображает полную стоимость клипа
        /// </summary>
        private void DisplayFullPrice() => fullPrice.text = $"СТОИМОСТЬ: {_fullPrice}";

        /// <summary>
        /// Кэшируем самые новые треки игрока на которые еще не снимался клип
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
            
            directorSwitcher.InstantiateElements(directorsList.Select(e => e.Name));
            operatorSwitcher.InstantiateElements(operatorsList.Select(e => e.Name));
            
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