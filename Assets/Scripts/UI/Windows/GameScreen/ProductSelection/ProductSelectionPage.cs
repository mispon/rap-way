using Enums;
using Core.Analytics;

namespace UI.Windows.GameScreen.ProductSelection
{
    public class ProductSelectionPage : Page
    {
        protected override void AfterShow(object ctx = null)
        {
            AnalyticsManager.LogEvent(FirebaseGameEvents.ProductionsClick);
        }
    }
}