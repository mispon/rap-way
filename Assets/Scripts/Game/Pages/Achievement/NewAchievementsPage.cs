using System.Collections.Generic;
using System.Linq;
using Game.Notifications;
using UnityEngine;
using UnityEngine.UI;
using Achiev = Models.Player.Achievement;

namespace Game.Pages.Achievement
{
    /// <summary>
    /// Страница отображения получения новой ачивки.
    /// Если получено несколько ачивок одним действием, то они сохраняются в списке.
    /// Закрытие странциы приводит к показу новой ачивки, если такова висит в списке, либо же к закрытию, если список пуст
    /// </summary>
    public class NewAchievementsPage: Page
    {
        [Header("Анимация окна")]
        [SerializeField] private Animation windowAnimation;

        [Header("Контроллы текста")]
        [SerializeField] private Text achievementText;
        [SerializeField] private Text descriptionText;

        /// <summary>
        /// Список пар строк: наименование ачивки и ее описание
        /// </summary>
        private List<KeyValuePair<string, string>> _achievementDescriptions = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Добавляет новую ачивку в очередь и запускает ее отображение, если страница закрыта
        /// </summary>
        public void ShowNewAchievement(string achievement, string description)
        {
            _achievementDescriptions.Add(new KeyValuePair<string, string>(achievement, description));

            if (!gameObject.activeSelf)
                Open();
        }

        #region PAGE EVENTS
        protected override void BeforePageOpen()
        {
            transform.localScale = Vector3.zero;

            var achievementDescription = _achievementDescriptions[0];
            achievementText.text = achievementDescription.Key;
            descriptionText.text = achievementDescription.Value;
        }

        protected override void AfterPageOpen()
        {
            windowAnimation.Play();
        }

        protected override void BeforePageClose()
        {
            NotificationManager.Instance.UnlockIndependentQueue();
        }

        protected override void AfterPageClose()
        {
            achievementText.text = "";
            descriptionText.text = "";
            _achievementDescriptions.RemoveAt(0);

            transform.localScale = Vector3.zero;

            if (_achievementDescriptions.Count > 0)
                Open();
        }
        #endregion
    }
}
