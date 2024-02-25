using System.Collections;
using Firebase.Analytics;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using UI.Enums;
using UnityEngine;

namespace Scenes
{
    public class IntroManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
            InitFirebase();
            yield return new WaitForSeconds(3);
            
            SceneMessageBroker.Instance.Publish(new SceneLoadMessage
            {
                SceneType = SceneType.MainMenu
            });
        }

        private static void InitFirebase()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) 
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
                } 
                else 
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }
    }
}