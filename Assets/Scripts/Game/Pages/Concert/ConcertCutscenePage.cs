using System;
using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Concert
{
    /// <summary>
    /// Страница катсцены концерта
    /// </summary>
    public class ConcertCutscenePage : Page
    {
        [Header("Анимации флекса")] [SerializeField]
        private GameObject[] flexingObjects;

        [Header("Пропуск катсцены")] [SerializeField]
        private Button skipButton;

        private int _flexingObjectIndex = -1;
        private ConcertInfo _concert;

        private void Start()
        {
            skipButton.onClick.AddListener(Close);
        }

        /// <summary>
        /// Открытие страницы с передачей информации о проведенном концерте
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            Open();
        }

        /// <summary>
        /// Влючаем нужную анимацию в зависимости от заполненности зала 
        /// </summary>
        private void FillDanceFloor()
        {
            var occupancyRatio = _concert.TicketsSold / (float) _concert.LocationCapacity;
            var locationOccupancyRatio = 1 / (float) flexingObjects.Length;
            _flexingObjectIndex = Mathf.FloorToInt(occupancyRatio / locationOccupancyRatio);

            flexingObjects[_flexingObjectIndex].SetActive(true);
        }

        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            FillDanceFloor();
        }

        protected override void AfterPageClose()
        {
            flexingObjects[_flexingObjectIndex].SetActive(false);
            _flexingObjectIndex = -1;
            _concert = null;
        }

        #endregion
    }
}