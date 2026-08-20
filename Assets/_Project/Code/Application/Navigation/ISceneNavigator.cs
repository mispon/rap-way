using System.Threading;
using System.Threading.Tasks;

namespace RapWay.Application.Navigation
{
    public interface ISceneNavigator
    {
        ValueTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken);
    }
}
