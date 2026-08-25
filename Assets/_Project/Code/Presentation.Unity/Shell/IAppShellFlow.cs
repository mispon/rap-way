using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Domain.State;

namespace RapWay.Presentation.Unity.Shell
{
    public interface IAppShellFlow
    {
        void ShowSplash();

        UniTask ShowMainMenuAsync(CancellationToken cancellationToken);

        void ShowHud();

        void UpdateHud(GameState state);
    }
}
