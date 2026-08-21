using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public readonly struct AudienceSegment : IEquatable<AudienceSegment>
    {
        public AudienceSegment(StableId groupId, long fanCount)
        {
            if (!groupId.IsValid)
            {
                throw new ArgumentException("Audience group ID must be valid.", nameof(groupId));
            }

            if (fanCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fanCount));
            }

            GroupId = groupId;
            FanCount = fanCount;
        }

        public StableId GroupId { get; }

        public long FanCount { get; }

        public bool Equals(AudienceSegment other)
        {
            return GroupId == other.GroupId && FanCount == other.FanCount;
        }

        public override bool Equals(object obj)
        {
            return obj is AudienceSegment other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GroupId, FanCount);
        }
    }
}
