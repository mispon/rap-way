using System;
using System.Collections.Generic;
using RapWay.Domain.Events;

namespace RapWay.Application.Commands
{
    public sealed class CommandResult
    {
        private static readonly IDomainEvent[] NoEvents = Array.Empty<IDomainEvent>();

        private CommandResult(
            bool isSuccess,
            CommandFailure? failure,
            long stateRevision,
            IReadOnlyList<IDomainEvent> committedEvents)
        {
            IsSuccess = isSuccess;
            Failure = failure;
            StateRevision = stateRevision;
            CommittedEvents = committedEvents;
        }

        public bool IsSuccess { get; }

        public CommandFailure? Failure { get; }

        public long StateRevision { get; }

        public IReadOnlyList<IDomainEvent> CommittedEvents { get; }

        internal static CommandResult Succeeded(long stateRevision, IReadOnlyList<IDomainEvent> committedEvents)
        {
            return new CommandResult(true, null, stateRevision, committedEvents);
        }

        internal static CommandResult Rejected(CommandFailure failure, long stateRevision)
        {
            return new CommandResult(false, failure, stateRevision, NoEvents);
        }
    }
}
