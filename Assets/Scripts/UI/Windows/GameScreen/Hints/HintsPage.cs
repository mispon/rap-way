using Enums;
using Firebase.Analytics;
using UI.Windows.GameScreen;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Hints
{
    public class HintsPage : Page
    {
        [SerializeField] private Text info;

        /// <summary>
        /// Показать страницу обучения
        /// </summary>
        public void Show(string key, string infoText)
        {
            if (key == "tutorial_on_start")
            {
                FirebaseAnalytics.LogEvent(FirebaseGameEvents.FirstHintOK);
            }
            
            info.text = infoText;
            Open();
        }
    }
}