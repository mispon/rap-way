using System.Collections;
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
            yield return new WaitForSeconds(loadingTime);
            SceneManager.Instance.LoadMainScene();
        }
    }
}
