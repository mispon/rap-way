using Enums;
using Firebase.Analytics;

namespace UI.Windows.GameScreen.ProductSelection 
{
    public class ProductSelectionPage : Page
    {
        protected override void AfterShow()
        {
            base.AfterShow();
            FirebaseAnalytics.LogEvent(FirebaseGameEvents.ProductionsClick);
        }
    }
}