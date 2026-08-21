using System;
using System.Globalization;
using RapWay.Application.Commands;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Events;
using RapWay.Domain.State;

namespace RapWay.Application.Activities
{
    public sealed class StartActivityCommandHandler : ICommandHandler<StartActivityCommand>
    {
        private readonly IActivityDefinitionLookup _definitions;

        public StartActivityCommandHandler(IActivityDefinitionLookup definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        public CommandExecution Execute(GameState state, StartActivityCommand command)
        {
            if (!command.DefinitionId.IsValid)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.InvalidDefinitionId);
            }

            if (state.ActiveActivity != null)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.ActivityAlreadyActive);
            }

            if (!_definitions.TryGet(command.DefinitionId, out ActivityDefinition definition))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.DefinitionNotFound);
            }

            if (!definition.SupportsDuration(command.DurationHours))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.InvalidDuration);
            }

            if (!state.Calendar.CanAdvance(command.DurationHours))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.CalendarLimitExceeded);
            }

            StableId instanceId = CreateInstanceId(state.Revision);
            state.BeginActivity(new ActivitySessionState(
                instanceId,
                definition.Id,
                state.Calendar.TotalHours,
                command.DurationHours));

            return CommandExecution.Succeeded(new ActivityStarted(instanceId, definition.Id, command.DurationHours));
        }

        private static StableId CreateInstanceId(long stateRevision)
        {
            return StableId.Create(
                "activity." + checked(stateRevision + 1).ToString(CultureInfo.InvariantCulture));
        }
    }
}
