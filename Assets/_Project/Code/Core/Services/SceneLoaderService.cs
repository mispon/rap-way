using Cysharp.Threading.Tasks;
using RapWay.Core.SaveSystem.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RapWay.Core.Services
{
    public class SceneLoaderService
    {
        private readonly SaveLoadService _saveLoadService;

        public SceneLoaderService(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }
        
        public async UniTask LoadSceneAsync(string sceneName)
        {
            // Здесь можно включить экран загрузки (Loading Screen UI)

            await _saveLoadService.SaveGameAsync();
            await SceneManager.LoadSceneAsync(sceneName);
            
            // Здесь можно выключить экран загрузки
        }
    }
}