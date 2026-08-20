using Cysharp.Threading.Tasks;
using RapWay.Application.Navigation;
using RapWay.Application.Session;
using RapWay.Presentation.Unity.Shell;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.App.Boot
{
    public class BootController : IStartable
    {
        private readonly ISceneNavigator _sceneNavigator;
        private readonly IAppShellFlow _appShellFlow;
        private readonly IGameSessionLaunchRequest _launchRequest;

        public BootController(
            ISceneNavigator sceneNavigator,
            IAppShellFlow appShellFlow,
            IGameSessionLaunchRequest launchRequest)
        {
            _sceneNavigator = sceneNavigator;
            _appShellFlow = appShellFlow;
            _launchRequest = launchRequest;
        }
        
        public void Start()
        {
            RunBootSequence().Forget();
        }
        
        private async UniTaskVoid RunBootSequence()
        {
            Debug.Log("Game Booting...");

            _appShellFlow.ShowSplash();
            await UniTask.Delay(3000);

            _launchRequest.Request(GameSessionLaunchMode.MainMenu);
            await _sceneNavigator.LoadSceneAsync("Game", default);
        }
    }
}
