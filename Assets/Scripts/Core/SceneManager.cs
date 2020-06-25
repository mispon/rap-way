using UnityEngine;
using Utils;

namespace Core
{
    /// <summary>
    /// Логика управления загрузкой сцен
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {
        [Header("Названия сцен")]
        [SerializeField] private string mainSceneName;
        [SerializeField] private string gameSceneName;

        /// <summary>
        /// Загружает сцену главного меню
        /// </summary>
        public void LoadMainScene() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
        
        /// <summary>
        /// Загружает игровую сцену
        /// </summary>
        public void LoadGameScene() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
}