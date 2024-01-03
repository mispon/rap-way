using Core;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Tutorial
{
    public class TutorialPage : Page
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