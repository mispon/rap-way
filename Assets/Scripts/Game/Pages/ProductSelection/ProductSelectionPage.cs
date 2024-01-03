using Core;
using Firebase.Analytics;

namespace Game.Pages.ProductSelection {
    /// <summary>
    /// Страница выбора активности
    /// </summary>
    public class ProductSelectionPage : Page
    {
        protected override void AfterPageOpen()
        {
            base.AfterPageOpen();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ProductionsClick);
        }
    }
}