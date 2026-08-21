using System;
using RapWay.Application.Commands;
using RapWay.Domain.Activities;
using RapWay.Domain.Events;
using RapWay.Domain.Numerics;
using RapWay.Domain.State;

namespace RapWay.Application.Activities
{
    public sealed class InterruptActivityCommandHandler : ICommandHandler<InterruptActivityCommand>
    {
        private readonly IActivityDefinitionLookup _definitions;

        public InterruptActivityCommandHandler(IActivityDefinitionLookup definitions)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
        }

        public CommandExecution Execute(GameState state, InterruptActivityCommand command)
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

            if (!definition.CanInterrupt)
            {
                return CommandExecution.Rejected(ActivityCommandFailures.CannotInterrupt);
            }

            Money payment = definition.CalculatePayment(activity.ElapsedHours);
            state.Character.Credit(payment);
            state.InterruptActiveActivity();

            return CommandExecution.Succeeded(new ActivityInterrupted(
                activity.InstanceId,
                activity.DefinitionId,
                activity.ElapsedHours,
                payment));
        }
    }
}
