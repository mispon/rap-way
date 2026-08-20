using System;
using System.Threading;
using System.Threading.Tasks;
using RapWay.Domain.State;

namespace RapWay.Application.Persistence
{
    public interface IGameSaveStore
    {
        Task SaveAsync(GameState stateSnapshot, DateTime savedAtUtc, CancellationToken cancellationToken);

        Task<GameLoadResult> LoadAsync(CancellationToken cancellationToken);
    }
}
