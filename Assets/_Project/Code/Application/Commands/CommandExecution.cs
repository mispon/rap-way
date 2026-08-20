using System;
using System.Collections.Generic;
using RapWay.Domain.Events;

namespace RapWay.Application.Commands
{
    public sealed class CommandExecution
    {
        private static readonly IDomainEvent[] NoEvents = Array.Empty<IDomainEvent>();

        private CommandExecution(bool isSuccess, CommandFailure? failure, IReadOnlyList<IDomainEvent> events)
        {
            IsSuccess = isSuccess;
            Failure = failure;
            Events = events;
        }

        public bool IsSuccess { get; }

        public CommandFailure? Failure { get; }

        public IReadOnlyList<IDomainEvent> Events { get; }

        public static CommandExecution Succeeded(params IDomainEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            IDomainEvent[] copy = new IDomainEvent[events.Length];
            for (int index = 0; index < events.Length; index++)
            {
                copy[index] = events[index] ?? throw new ArgumentException("Committed events cannot contain null.", nameof(events));
            }

            return new CommandExecution(true, null, Array.AsReadOnly(copy));
        }

        public static CommandExecution Rejected(CommandFailure failure)
        {
            return new CommandExecution(false, failure, NoEvents);
        }
    }
}
