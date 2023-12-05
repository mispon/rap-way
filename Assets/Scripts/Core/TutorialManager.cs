using Core.Interfaces;
using Game;
using Game.Pages.Tutorial;
using Localization;
using UnityEngine;
using Utils;

namespace Core
{
    public class TutorialManager : Singleton<TutorialManager>, IStarter
    {
        [SerializeField] private TutorialPage page;
        
        public void OnStart()
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