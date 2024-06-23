using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Achievement
{
    public class NewAchievementsPage : Page
    {
        [SerializeField] private Text achievementText;

        private readonly Queue<string> _achievements = new();

        public void ShowNewAchievement(string achievement)
        {
            _achievements.Enqueue(achievement);

            try
            {
                if (!gameObject.activeSelf)
                {
                    Show();
                }
            } catch (MissingReferenceException) { }
        }

        protected override void BeforeShow(object ctx = null)
        {
            achievementText.text = _achievements.Dequeue();
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