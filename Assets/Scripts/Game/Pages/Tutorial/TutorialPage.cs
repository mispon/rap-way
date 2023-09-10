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
        public void Show(string infoText)
        {
            info.text = infoText;
            Open();
        }
    }
}