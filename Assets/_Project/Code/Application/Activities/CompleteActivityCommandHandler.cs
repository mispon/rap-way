using System;
using RapWay.Application.Commands;
using RapWay.Domain.Activities;
using RapWay.Domain.Events;
using RapWay.Domain.Numerics;
using RapWay.Domain.State;

namespace RapWay.Application.Activities
{
    public sealed class CompleteActivityCommandHandler : ICommandHandler<CompleteActivityCommand>
    {
        private readonly IActivityDefinitionLookup _definitions;

        public CompleteActivityCommandHandler(IActivityDefinitionLookup definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        public CommandExecution Execute(GameState state, CompleteActivityCommand command)
        {
            ActivitySessionState? activity = state.ActiveActivity;
            if (activity == null)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.NoActiveActivity);
            }

            if (!activity.IsComplete)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.ActivityIsNotComplete);
            }

            if (!_definitions.TryGet(activity.DefinitionId, out ActivityDefinition definition))
            {
                return CommandExecution.Rejected(ActivityCommandFailures.DefinitionNotFound);
            }

            Money payment = definition.CalculatePayment(activity.ElapsedHours);
            state.Character.Credit(payment);
            state.CompleteActiveActivity();

            return CommandExecution.Succeeded(new ActivityCompleted(
                activity.InstanceId,
                activity.DefinitionId,
                activity.ElapsedHours,
                payment));
        }
    }
}
