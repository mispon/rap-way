using System.Collections;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Экран "интро"
    /// </summary>
    public class IntroManager : MonoBehaviour
    {
        [SerializeField] private int delay = 3;
        [SerializeField] private string mainSceneName = "Menu";

        private IEnumerator Start()
        {
            // todo:
            // дождаться загрузки рекламы
            // запустить рекламный ролик
            // загрузить главное меню
            yield return new WaitForSeconds(delay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
        }
    }
}
