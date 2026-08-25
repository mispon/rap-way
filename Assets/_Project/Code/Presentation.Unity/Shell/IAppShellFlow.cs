using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Domain.State;

namespace RapWay.Presentation.Unity.Shell
{
    public interface IAppShellFlow
    {
        event Action ActivitySelectionRequested;

        void ShowSplash();

        UniTask ShowMainMenuAsync(CancellationToken cancellationToken);

        void ShowHud();

        void UpdateHud(GameState state);

        void RequestActivitySelection();
    }
}
