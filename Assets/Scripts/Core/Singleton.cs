using UnityEngine;

namespace Core
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        [SerializeField] [Header("Keep object between scenes")]
        protected bool dontDestroy;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetInstance();
                Initialize();
            } else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Initialize() { }
        protected virtual void Dispose()    { }

        private T GetInstance()
        {
            var instance = FindAnyObjectByType<T>() ?? CreateInstance();

            if (dontDestroy)
            {
                DontDestroyOnLoad(instance);
            }

            return instance;
        }

        private static T CreateInstance()
        {
            var singleton = new GameObject($"{nameof(T)} (Singleton)");
            return singleton.AddComponent<T>();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}