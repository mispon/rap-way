using Enums;
using Firebase.Analytics;
using UI.Windows.GameScreen;

namespace UI.Windows.Pages.ProductSelection {
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