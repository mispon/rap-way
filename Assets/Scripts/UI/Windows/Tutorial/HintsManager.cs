using Core;
using Core.Localization;
using Core.OrderedStarter;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Windows.Pages.Hints;
using UnityEngine;

namespace UI.Windows.Tutorial
{
    public class HintsManager : Singleton<HintsManager>, IStarter
    {
        [SerializeField] private HintsPage page;
        
        public void OnStart()
        {
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
        }

        public void ShowHint(string key)
        {
            bool ok = GameManager.Instance.ShowedHints.Add(key);
            if (!ok)
                return;

            var info = LocalizationManager.Instance.Get(key);
            page.Show(key, info);
        }
    }
}