using Core;
using Core.Localization;
using Core.OrderedStarter;
using UI.Windows.Pages.Tutorial;
using UnityEngine;

namespace Game.Tutorial
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
            page.Show(key, info);
        }
    }
}