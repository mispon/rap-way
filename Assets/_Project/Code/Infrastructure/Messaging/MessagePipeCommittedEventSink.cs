using System;
using System.Collections.Generic;
using MessagePipe;
using RapWay.Application.Events;
using RapWay.Domain.Events;

namespace RapWay.Infrastructure.Messaging
{
    public sealed class MessagePipeCommittedEventSink : ICommittedEventSink
    {
        private readonly IPublisher<IDomainEvent> _publisher;

        public MessagePipeCommittedEventSink(IPublisher<IDomainEvent> publisher)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public void Publish(IReadOnlyList<IDomainEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            for (int index = 0; index < events.Count; index++)
            {
                IDomainEvent domainEvent = events[index] ??
                                           throw new ArgumentException("Committed events cannot contain null.", nameof(events));
                _publisher.Publish(domainEvent);
            }
        }
    }
}
