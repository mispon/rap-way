using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Паттерн "одиночка"
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        public static T Instance { get; private set; }

        [SerializeField, Header("Нужно ли сохранять объект при переключении сцен")]
        protected bool dontDestroy;

        protected virtual void Awake() 
        {
            if (Instance == null) 
            {
                Instance = GetInstance();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Returns a single object
        /// </summary>
        private T GetInstance() 
        {
            var instance = FindObjectOfType<T>() ?? CreateInstance();
            if (dontDestroy) DontDestroyOnLoad(instance);
            return instance;
        }

        /// <summary>
        /// Creates a new object on stage
        /// </summary>
        private static T CreateInstance() 
        {
            var singleton = new GameObject($"{nameof(T)} (Singleton)");
            return singleton.AddComponent<T>();
        }
    }
}