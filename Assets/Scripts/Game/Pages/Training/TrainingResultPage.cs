using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Training
{
    /// <summary>
    /// Страница результатов тренировки
    /// </summary>
    public class TrainingResultPage : Page
    {
        [SerializeField] private Text info;

        /// <summary>
        /// Показывает окно результата 
        /// </summary>
        public void Show(string message)
        {
            info.text = message;
            Open();
        }
    }
}