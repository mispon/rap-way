using System;
using RapWay.Domain.Common;
using RapWay.Domain.Numerics;

namespace RapWay.Domain.Events
{
    public sealed class ActivityInterrupted : IDomainEvent
    {
        public ActivityInterrupted(StableId instanceId, StableId definitionId, int completedHours, Money payment)
        {
            if (!instanceId.IsValid || !definitionId.IsValid)
            {
                throw new ArgumentException("Activity event IDs must be valid.");
            }

            if (completedHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(completedHours));
            }

            if (payment.MinorUnits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(payment));
            }

            InstanceId = instanceId;
            DefinitionId = definitionId;
            CompletedHours = completedHours;
            Payment = payment;
        }

        public StableId InstanceId { get; }

        public StableId DefinitionId { get; }

        public int CompletedHours { get; }

        public Money Payment { get; }
    }
}
