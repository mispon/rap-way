using System.Collections.Generic;
using Core;
using Core.Localization;
using Core.OrderedStarter;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;

namespace UI.Windows.Tutorial
{
    public class HintsManager : Singleton<HintsManager>, IStarter
    {
        public void OnStart()
        {
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
        }

        public void ShowHint(string key, object pageCtx = null)
        {
            bool ok = GameManager.Instance.ShowedHints.Add(key);
            if (!ok)
                return;

            var info = LocalizationManager.Instance.Get(key);
            
            MsgBroker.Instance.Publish(new WindowControlMessage
            {
                Type = WindowType.Hints,
                Context = new Dictionary<string, object>
                {
                    ["hint_key"]  = key,
                    ["hint_text"] = info,
                    ["page_ctx"]  = pageCtx
                }
            });
        }
    }
}