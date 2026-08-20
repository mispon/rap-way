using System.Collections.Generic;
using RapWay.Domain.Events;

namespace RapWay.Application.Events
{
    public interface ICommittedEventSink
    {
        void Publish(IReadOnlyList<IDomainEvent> events);
    }
}
