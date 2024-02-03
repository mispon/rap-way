using Core;
using Core.Localization;
using Core.OrderedStarter;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UI.Windows.Pages.Hints;
using UnityEngine;

namespace Game.Player
{
    public class HintsManager : Singleton<HintsManager>, IStarter
    {
        [SerializeField] private HintsPage page;
        
        public void OnStart()
        {
            UIMessageBroker.Instance.Publish(new TutorialWindowControlMessage());
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