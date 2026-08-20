using System.Threading;
using Cysharp.Threading.Tasks;

namespace RapWay.Presentation.Unity.Shell
{
    public interface IAppShellFlow
    {
        void ShowSplash();

        UniTask ShowMainMenuAsync(CancellationToken cancellationToken);

        void ShowHud();
    }
}
