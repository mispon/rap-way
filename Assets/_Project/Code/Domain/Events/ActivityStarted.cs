using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Events
{
    public sealed class ActivityStarted : IDomainEvent
    {
        public ActivityStarted(StableId instanceId, StableId definitionId, int durationHours)
        {
            if (!instanceId.IsValid || !definitionId.IsValid)
            {
                throw new ArgumentException("Activity event IDs must be valid.");
            }

            if (durationHours <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(durationHours));
            }

            InstanceId = instanceId;
            DefinitionId = definitionId;
            DurationHours = durationHours;
        }

        public StableId InstanceId { get; }

        public StableId DefinitionId { get; }

        public int DurationHours { get; }
    }
}
