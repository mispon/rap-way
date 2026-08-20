using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RapWay.Core.Services
{
    public class SceneLoaderService
    {
        public async UniTask LoadSceneAsync(string sceneName)
        {
            // Здесь можно включить экран загрузки (Loading Screen UI)

            await SceneManager.LoadSceneAsync(sceneName);
            
            // Здесь можно выключить экран загрузки
        }
    }
}
