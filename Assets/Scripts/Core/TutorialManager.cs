using Game;
using Game.Pages.Tutorial;
using Localization;
using UnityEditor.Callbacks;
using UnityEngine;
using Utils;

namespace Core
{
    public class TutorialManager : Singleton<TutorialManager>
    {
        [SerializeField] private TutorialPage page;
        
        private void Start()
        {
            ShowTutorial("tutorial_on_start");
        }

        public void ShowTutorial(string key)
        {
            if (GameManager.Instance.ShowedTutorials.Contains(key))
            {
                return;
            }

            GameManager.Instance.ShowedTutorials.Add(key);

            var info = LocalizationManager.Instance.Get(key);
            page.Show(info);
        }
    }
}