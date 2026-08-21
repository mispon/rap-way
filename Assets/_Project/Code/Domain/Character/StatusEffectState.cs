using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public readonly struct StatusEffectState : IEquatable<StatusEffectState>
    {
        public StatusEffectState(
            StableId instanceId,
            StableId definitionId,
            StableId sourceId,
            long appliedAtTotalHours,
            long? expiresAtTotalHours,
            int stacks)
        {
            if (!instanceId.IsValid || !definitionId.IsValid || !sourceId.IsValid)
            {
                throw new ArgumentException("Status effect IDs must be valid.");
            }

            if (appliedAtTotalHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(appliedAtTotalHours));
            }

            if (expiresAtTotalHours.HasValue && expiresAtTotalHours.Value < appliedAtTotalHours)
            {
                throw new ArgumentOutOfRangeException(nameof(expiresAtTotalHours));
            }

            if (stacks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stacks));
            }

            InstanceId = instanceId;
            DefinitionId = definitionId;
            SourceId = sourceId;
            AppliedAtTotalHours = appliedAtTotalHours;
            ExpiresAtTotalHours = expiresAtTotalHours;
            Stacks = stacks;
        }

        public StableId InstanceId { get; }

        public StableId DefinitionId { get; }

        public StableId SourceId { get; }

        public long AppliedAtTotalHours { get; }

        public long? ExpiresAtTotalHours { get; }

        public int Stacks { get; }

        public bool Equals(StatusEffectState other)
        {
            return InstanceId == other.InstanceId &&
                   DefinitionId == other.DefinitionId &&
                   SourceId == other.SourceId &&
                   AppliedAtTotalHours == other.AppliedAtTotalHours &&
                   ExpiresAtTotalHours == other.ExpiresAtTotalHours &&
                   Stacks == other.Stacks;
        }

        public override bool Equals(object obj)
        {
            return obj is StatusEffectState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InstanceId, DefinitionId, SourceId, AppliedAtTotalHours, ExpiresAtTotalHours, Stacks);
        }
    }
}
