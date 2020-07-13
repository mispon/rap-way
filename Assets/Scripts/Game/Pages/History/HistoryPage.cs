using System;
using System.Linq;
using Game.Pages.History.HistoryProduction;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.History
{
    /// <summary>
    /// Страница Истории
    /// </summary>
    public class HistoryPage: Page
    {
        /// <summary>
        /// Событие переключения на другой Production
        /// </summary>
        public event Action<HistoryProductionController> onFoldoutInfoShow = delegate { };
        
        [Header("Контролы управления Production")] 
        [SerializeField] private HistoryTrackController trackHistoryController;
        [Space, SerializeField] private HistoryAlbumController albumHistoryController;
        [Space, SerializeField] private HistoryClipController clipHistoryController;
        [Space, SerializeField] private HistoryConcertController concertHistoryController;

        [Header("Контроллер ScrollView")]
        [SerializeField] private HistoryScrollViewController scrollViewController;
        
        /// <summary>
        /// Перечисление всех контроллеров UI-элементов определенного типа Production
        /// </summary>
        private HistoryProductionController[] historyControllers => new HistoryProductionController[]
        {
            trackHistoryController,
            albumHistoryController,
            clipHistoryController,
            concertHistoryController
        };
        
        /// <summary>
        /// Вызывает событие переключения на другой Production
        /// </summary>
        public void ShowInfo(HistoryProductionController productionController)
        {
            onFoldoutInfoShow(productionController);
            RequestDisableElements(productionController);
        }

        /// <summary>
        /// Деактивация объектов-информации об экземлярах Production
        /// </summary>
        private void RequestDisableElements(HistoryProductionController requestController)
        {
            foreach (var productionController in historyControllers.Where(hc => hc != requestController))
                productionController.SetActiveElements(false);
        }

        protected override void BeforePageOpen()
        {
            foreach (var historyController in historyControllers)
                historyController.Initialize(this, scrollViewController);
            
            trackHistoryController.Show();
        }
    }
}