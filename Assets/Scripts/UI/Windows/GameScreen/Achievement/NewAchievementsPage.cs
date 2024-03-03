using System.Collections.Generic;
using Game.Notifications;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Achievement
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
        private readonly Queue<string> _achievements = new();

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
                    Show();
                }    
            } catch(MissingReferenceException) {}
        }

        protected override void BeforeShow()
        {
            achievementText.text = _achievements.Dequeue();
        }

        protected override void BeforeHide()
        {
            NotificationManager.Instance.UnlockIndependentQueue();
        }

        protected override void AfterHide()
        {
            achievementText.text = string.Empty;

            if (_achievements.Count > 0)
            {
                Show();
            }
        }
    }
}