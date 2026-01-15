using Cysharp.Threading.Tasks;
using RapWay.Core.Services;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.App.Boot
{
    public class BootController : IStartable
    {
        private readonly SceneLoaderService _sceneLoader;

        public BootController(SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }
        
        public void Start()
        {
            RunBootSequence().Forget();
        }
        
        private async UniTaskVoid RunBootSequence()
        {
            Debug.Log("Game Booting...");
            
            // show studio logo and wait
            // todo: show logo
            await UniTask.Delay(3000);
            
            // go to main menu
            await _sceneLoader.LoadSceneAsync("MainMenu");
        }
    }
}