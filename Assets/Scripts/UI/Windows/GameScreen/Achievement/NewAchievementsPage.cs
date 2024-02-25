using System.Collections.Generic;
using Game.Notifications;
using UI.Windows.GameScreen;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Pages.Achievement
{
    /// <summary>
    /// Страница получения новой ачивки
    /// </summary>
    public class NewAchievementsPage: Page
    {
        [SerializeField] private Text achievementText;

        /// <summary>
        /// Список пар строк: наименование ачивки и ее описание
        /// </summary>
        private readonly Queue<string> _achievements = new Queue<string>();

        /// <summary>
        /// Добавляет новую ачивку в очередь и запускает ее отображение, если страница закрыта
        /// </summary>
        public void ShowNewAchievement(string achievement)
        {
            _achievements.Enqueue(achievement);

            try
            {
                if (!gameObject.activeSelf)
                {
                    Open();
                }    
            } catch(MissingReferenceException) {}
        }

        protected override void BeforePageOpen()
        {
            achievementText.text = _achievements.Dequeue();
        }

        protected override void BeforePageClose()
        {
            NotificationManager.Instance.UnlockIndependentQueue();
        }

        protected override void AfterPageClose()
        {
            achievementText.text = string.Empty;

            if (_achievements.Count > 0)
            {
                Open();
            }
        }
    }
}