using RapWay.Application.Commands;
using RapWay.Domain.Common;

namespace RapWay.Application.Activities
{
    public sealed class StartActivityCommand : IGameCommand
    {
        public StartActivityCommand(StableId definitionId, int durationHours)
        {
            DefinitionId = definitionId;
            DurationHours = durationHours;
        }

        public StableId DefinitionId { get; }

        public int DurationHours { get; }
    }
}
