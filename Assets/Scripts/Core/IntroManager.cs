using System.Collections;
using Firebase.Analytics;
using Scenes;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Экран "интро"
    /// </summary>
    public class IntroManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
            InitFirebase();
            yield return new WaitForSeconds(3);
            
            ScenesController.Instance.MessageBroker.Publish(new SceneLoadMessage
            {
                SceneType = SceneTypes.MainMenu
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