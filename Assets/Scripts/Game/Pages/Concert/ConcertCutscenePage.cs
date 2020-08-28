using System;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Concert
{
    public class ConcertCutscenePage : Page
    {
        [Header("Анимации флекса")] 
        [SerializeField] private GameObject[] flexingObjects;
        
        [Header("Пропуск катсцены")] 
        [SerializeField] private Button skipButton;

        private int _flexingIndex = -1;
        private ConcertInfo _concert;
        
        /// <summary>
        /// Открытие страницы с передачей информации о проведенном концерте
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            Open();
        }

        private void Start()
        {
            skipButton.onClick.AddListener(Close);
        }

        /// <summary>
        /// Влючаем нужную анимацию в зависимости от заполненности зала 
        /// </summary>
        private void FillDanceFloor()
        {
            var occupancyRatio = _concert.TicketsSold / (float) _concert.LocationCapacity;
            float locationOccupancyRatio = 1 / (float) flexingObjects.Length;
            _flexingIndex = Mathf.FloorToInt(occupancyRatio / locationOccupancyRatio);

            flexingObjects[_flexingIndex].SetActive(true);
        }

        #region PAGE CALLBACKS
        
        protected override void BeforePageOpen()
        {
            FillDanceFloor();
        }

        protected override void AfterPageClose()
        {
            flexingObjects[_flexingIndex].SetActive(false);
            _flexingIndex = -1;
            _concert = null;
        }
        
        #endregion
    }
}