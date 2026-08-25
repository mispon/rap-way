using RapWay.Application.Commands;
using RapWay.Domain.Common;
using RapWay.Domain.State;

namespace RapWay.Application.Activities
{
    public interface IActivityLoop
    {
        bool TryGetStateSnapshot(out GameState stateSnapshot);

        CommandResult Start(StableId definitionId, int durationHours);

        CommandResult AdvanceHour();

        CommandResult Complete();

        CommandResult Interrupt();
    }
}
