using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Events
{
    public sealed class ActivityHourElapsed : IDomainEvent
    {
        public ActivityHourElapsed(StableId instanceId, StableId definitionId, int elapsedHours)
        {
            if (!instanceId.IsValid || !definitionId.IsValid)
            {
                throw new ArgumentException("Activity event IDs must be valid.");
            }

            if (elapsedHours <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedHours));
            }

            InstanceId = instanceId;
            DefinitionId = definitionId;
            ElapsedHours = elapsedHours;
        }

        public StableId InstanceId { get; }

        public StableId DefinitionId { get; }

        public int ElapsedHours { get; }
    }
}
