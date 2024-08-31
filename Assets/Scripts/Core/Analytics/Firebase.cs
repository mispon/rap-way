#if UNITY_ANDROID
using Firebase.Analytics;
#elif UNITY_IPHONE
// TODO
#else
// disabled
#endif

namespace Core.Analytics
{
    public static class AnalyticsManager
    {
        public static void Init()
        {
#if UNITY_ANDROID
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
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
#elif UNITY_IPHONE
            // TODO
#else
            // disabled
#endif
        }

        public static void LogEvent(string eventType)
        {
#if UNITY_ANDROID
            FirebaseAnalytics.LogEvent(eventType);
#elif UNITY_IPHONE
            // TODO
#else
            // disabled
#endif
        }
    }
}