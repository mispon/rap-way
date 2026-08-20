using System.Collections.Generic;
using RapWay.Domain.Events;

namespace RapWay.Application.Events
{
    public sealed class DiscardCommittedEventSink : ICommittedEventSink
    {
        public static DiscardCommittedEventSink Instance { get; } = new();

        private DiscardCommittedEventSink()
        {
        }

        public void Publish(IReadOnlyList<IDomainEvent> events)
        {
        }
    }
}
