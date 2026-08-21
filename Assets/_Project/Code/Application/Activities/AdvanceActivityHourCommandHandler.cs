using System;
using RapWay.Application.Commands;
using RapWay.Domain.Activities;
using RapWay.Domain.Events;
using RapWay.Domain.State;

namespace RapWay.Application.Activities
{
    public sealed class AdvanceActivityHourCommandHandler : ICommandHandler<AdvanceActivityHourCommand>
    {
        private readonly IActivityDefinitionLookup _definitions;

        public AdvanceActivityHourCommandHandler(IActivityDefinitionLookup definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        public CommandExecution Execute(GameState state, AdvanceActivityHourCommand command)
        {
            ActivitySessionState? activity = state.ActiveActivity;
            if (activity == null)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.NoActiveActivity);
            }

            if (activity.IsComplete)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.ActivityIsComplete);
            }

            if (!_definitions.TryGet(activity.DefinitionId, out ActivityDefinition definition))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.DefinitionNotFound);
            }

            if (!state.Calendar.CanAdvance(1))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.CalendarLimitExceeded);
            }

            state.ApplyActiveActivityHour(definition.HourlyEffects);
            ActivitySessionState advancedActivity = state.ActiveActivity!;
            return CommandExecution.Succeeded(new ActivityHourElapsed(
                advancedActivity.InstanceId,
                advancedActivity.DefinitionId,
                advancedActivity.ElapsedHours));
        }
    }
}
