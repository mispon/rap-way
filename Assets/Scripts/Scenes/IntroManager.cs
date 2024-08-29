using Core.Analytics;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using System.Collections;
using UI.Enums;
using UnityEngine;

namespace Scenes
{
    public class IntroManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
            AnalyticsManager.Init();
            yield return new WaitForSeconds(3);

            SceneMessageBroker.Instance.Publish(new SceneLoadMessage
            {
                SceneType = SceneType.MainMenu
            });
        }
    }
}