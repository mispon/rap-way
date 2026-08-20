using RapWay.Domain.State;

namespace RapWay.Application.Persistence
{
    public interface IGameStateSnapshotSource
    {
        bool TryGetStateSnapshot(out GameState stateSnapshot);
    }
}
