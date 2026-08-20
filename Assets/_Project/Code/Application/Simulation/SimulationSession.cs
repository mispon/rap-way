using System;
using System.Collections.Generic;
using RapWay.Application.Commands;
using RapWay.Application.Events;
using RapWay.Domain.Events;
using RapWay.Domain.State;

namespace RapWay.Application.Simulation
{
    public sealed class SimulationSession
    {
        private readonly ICommittedEventSink _eventSink;
        private GameState _state;

        public SimulationSession(GameState initialState, ICommittedEventSink eventSink)
        {
            _state = initialState?.Copy() ?? throw new ArgumentNullException(nameof(initialState));
            _eventSink = eventSink ?? throw new ArgumentNullException(nameof(eventSink));
        }

        public GameState GetStateSnapshot()
        {
            return _state.Copy();
        }

        public CommandResult Execute<TCommand>(TCommand command, ICommandHandler<TCommand> handler)
            where TCommand : IGameCommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            long previousRevision = _state.Revision;
            GameState workingState = _state.Copy();
            CommandExecution execution = handler.Execute(workingState, command) ??
                                         throw new InvalidOperationException("A command handler returned no execution result.");

            if (!execution.IsSuccess)
            {
                if (!execution.Failure.HasValue)
                {
                    throw new InvalidOperationException("A rejected command must provide a stable failure code.");
                }

                return CommandResult.Rejected(execution.Failure.Value, previousRevision);
            }

            workingState.Validate();
            workingState.CommitNextRevision(previousRevision);

            IReadOnlyList<IDomainEvent> committedEvents = CopyEvents(execution.Events);
            _state = workingState.Copy();
            _eventSink.Publish(committedEvents);

            return CommandResult.Succeeded(_state.Revision, committedEvents);
        }

        private static IReadOnlyList<IDomainEvent> CopyEvents(IReadOnlyList<IDomainEvent> events)
        {
            IDomainEvent[] copy = new IDomainEvent[events.Count];
            for (int index = 0; index < events.Count; index++)
            {
                copy[index] = events[index] ?? throw new InvalidOperationException("A successful command produced a null event.");
            }

            return Array.AsReadOnly(copy);
        }
    }
}
