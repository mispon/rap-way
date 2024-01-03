using System.Collections;
using Firebase.Analytics;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Экран "интро"
    /// </summary>
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] private int loadingTime = 3;

        private IEnumerator Start()
        {
            InitFirebase();
            yield return new WaitForSeconds(loadingTime);
            SceneManager.Instance.LoadMainScene();
        }

        private static void InitFirebase()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) 
                {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                } 
                else 
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });

        }
    }
}