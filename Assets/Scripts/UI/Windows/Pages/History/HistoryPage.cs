using System;
using System.Linq;
using Enums;
using Firebase.Analytics;
using Game.Player;
using UI.Controls.ScrollViewController;
using UI.Windows.Pages.History.HistoryProduction;
using UnityEngine;

namespace UI.Windows.Pages.History
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
        [SerializeField] private HistoryAlbumController albumHistoryController;
        [SerializeField] private HistoryClipController clipHistoryController;
        [SerializeField] private HistoryConcertController concertHistoryController;

        [Header("Контроллер ScrollView")]
        [SerializeField] private ScrollViewController scrollViewController;
        
        protected override void AfterPageOpen()
        {
            HintsManager.Instance.ShowHint("tutorial_history");
        }
        
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
            {
                productionController.SetActiveElements(false);
            }
        }

        protected override void BeforePageOpen()
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.HistoryPageOpened);
            
            foreach (var historyController in historyControllers)
            {
                historyController.Initialize(this, scrollViewController);   
            }
            
            trackHistoryController.Show(silent: true);
        }
    }
}