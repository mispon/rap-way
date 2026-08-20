using Cysharp.Threading.Tasks;
using RapWay.Application.Session;
using RapWay.Presentation.Unity.Shell;
using VContainer.Unity;

namespace RapWay.App
{
    public class GameStartup : IStartable
    {
        private readonly IAppShellFlow _appShellFlow;
        private readonly IGameSessionLaunchRequest _launchRequest;

        public GameStartup(IAppShellFlow appShellFlow, IGameSessionLaunchRequest launchRequest)
        {
            _appShellFlow = appShellFlow;
            _launchRequest = launchRequest;
        }

        public void Start()
        {
            RunSceneEntryFlow().Forget();
        }

        private async UniTaskVoid RunSceneEntryFlow()
        {
            switch (_launchRequest.LastRequestedMode)
            {
                case GameSessionLaunchMode.MainMenu:
                    await _appShellFlow.ShowMainMenuAsync(default);
                    break;

                case GameSessionLaunchMode.Continue:
                case GameSessionLaunchMode.NewCareer:
                default:
                    _appShellFlow.ShowHud();
                    break;
            }
        }
    }
}
