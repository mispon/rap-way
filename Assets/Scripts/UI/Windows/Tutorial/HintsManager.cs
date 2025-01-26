using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Localization;
using Core.OrderedStarter;
using Game;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UnityEngine;

namespace UI.Windows.Tutorial
{
    public class HintsManager : Singleton<HintsManager>, IStarter
    {
        public void OnStart()
        {
            StartCoroutine(StartTutorial());
        }

        private static IEnumerator StartTutorial()
        {
            yield return new WaitForSeconds(1);
            MsgBroker.Instance.Publish(new TutorialWindowControlMessage());
        }

        public bool ShowHint(string key, object pageCtx = null)
        {
            var ok = GameManager.Instance.ShowedHints.Add(key);
            if (!ok)
            {
                return false;
            }

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

            return true;
        }
    }
}