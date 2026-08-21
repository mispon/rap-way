using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Activities
{
    public sealed class ActivitySessionState
    {
        public const int MaximumDurationHours = 24;

        public ActivitySessionState(
            StableId instanceId,
            StableId definitionId,
            long startedAtTotalHours,
            int durationHours,
            int elapsedHours = 0)
        {
            if (!instanceId.IsValid)
            {
                throw new ArgumentException("Activity instance ID must be valid.", nameof(instanceId));
            }

            if (!definitionId.IsValid)
            {
                throw new ArgumentException("Activity definition ID must be valid.", nameof(definitionId));
            }

            if (startedAtTotalHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startedAtTotalHours));
            }

            if (durationHours <= 0 || durationHours > MaximumDurationHours)
            {
                throw new ArgumentOutOfRangeException(nameof(durationHours));
            }

            if (elapsedHours < 0 || elapsedHours > durationHours)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedHours));
            }

            InstanceId = instanceId;
            DefinitionId = definitionId;
            StartedAtTotalHours = startedAtTotalHours;
            DurationHours = durationHours;
            ElapsedHours = elapsedHours;
        }

        public StableId InstanceId { get; }

        public StableId DefinitionId { get; }

        public long StartedAtTotalHours { get; }

        public int DurationHours { get; }

        public int ElapsedHours { get; private set; }

        public int RemainingHours => DurationHours - ElapsedHours;

        public bool IsComplete => ElapsedHours == DurationHours;

        internal void Advance(int hours)
        {
            if (hours <= 0 || hours > RemainingHours)
            {
                throw new InvalidOperationException("The activity cannot advance by the requested number of hours.");
            }

            ElapsedHours = checked(ElapsedHours + hours);
        }

        internal ActivitySessionState Copy()
        {
            return new ActivitySessionState(InstanceId, DefinitionId, StartedAtTotalHours, DurationHours, ElapsedHours);
        }

        internal void Validate()
        {
            _ = new ActivitySessionState(InstanceId, DefinitionId, StartedAtTotalHours, DurationHours, ElapsedHours);
        }
    }
}
