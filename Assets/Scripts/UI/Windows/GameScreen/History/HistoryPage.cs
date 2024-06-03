using System;
using System.Linq;
using Enums;
// using Firebase.Analytics;
using Sirenix.OdinInspector;
using UI.Controls.ScrollViewController;
using UI.Windows.GameScreen.History.HistoryProduction;
using UI.Windows.Tutorial;
using UnityEngine;

namespace UI.Windows.GameScreen.History
{
    public class HistoryPage: Page
    {
        public event Action<HistoryProductionController> onFoldoutInfoShow = delegate { };
        
        [BoxGroup("Controllers"), SerializeField] private HistoryTrackController trackHistoryController;
        [BoxGroup("Controllers"), SerializeField] private HistoryAlbumController albumHistoryController;
        [BoxGroup("Controllers"), SerializeField] private HistoryClipController clipHistoryController;
        [BoxGroup("Controllers"), SerializeField] private HistoryConcertController concertHistoryController;
        
        [BoxGroup("Scroll View"), SerializeField] private ScrollViewController scrollViewController;
        
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