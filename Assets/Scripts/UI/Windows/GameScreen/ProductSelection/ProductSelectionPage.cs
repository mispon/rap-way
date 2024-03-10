using Enums;
using Firebase.Analytics;

namespace UI.Windows.GameScreen.ProductSelection 
{
    public class ProductSelectionPage : Page
    {
        protected override void AfterShow(object ctx = null)
        {
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ProductionsClick);
        }
    }
}