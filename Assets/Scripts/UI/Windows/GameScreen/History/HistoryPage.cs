using System;
using System.Linq;
// using Firebase.Analytics;
using UI.Controls.ScrollViewController;
using UI.Windows.GameScreen.History.HistoryProduction;
using UI.Windows.Tutorial;
using UnityEngine;

namespace UI.Windows.GameScreen.History
{
    public class HistoryPage: Page
    {
        public event Action<HistoryProductionController> onFoldoutInfoShow = delegate { };
        
        [Header("Controllers")]
        [SerializeField] private HistoryTrackController trackHistoryController;
        [SerializeField] private HistoryAlbumController albumHistoryController;
        [SerializeField] private HistoryClipController clipHistoryController;
        [SerializeField] private HistoryConcertController concertHistoryController;
        
        [Header("Scroll View"), SerializeField] private ScrollViewController scrollViewController;
        
        private HistoryProductionController[] historyControllers => new HistoryProductionController[]
        {
            trackHistoryController,
            albumHistoryController,
            clipHistoryController,
            concertHistoryController
        };
        
        public void ShowInfo(HistoryProductionController productionController)
        {
            onFoldoutInfoShow(productionController);
            RequestDisableElements(productionController);
        }

        private void RequestDisableElements(HistoryProductionController requestController)
        {
            foreach (var productionController in historyControllers.Where(hc => hc != requestController))
            {
                productionController.SetActiveElements(false);
            }
        }

        protected override void BeforeShow(object ctx = null)
        {
            // FirebaseAnalytics.LogEvent(FirebaseGameEvents.HistoryPageOpened);
            
            foreach (var historyController in historyControllers)
            {
                historyController.Initialize(this, scrollViewController);   
            }
            
            trackHistoryController.Show(silent: true);
        }
        
        protected override void AfterShow(object ctx = null)
        {
            HintsManager.Instance.ShowHint("tutorial_history");
        }
    }
}