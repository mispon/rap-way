using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public readonly struct SkillProgress : IEquatable<SkillProgress>
    {
        public SkillProgress(StableId skillId, long experience, long? lastPracticedTotalHours)
        {
            if (!skillId.IsValid)
            {
                throw new ArgumentException("Skill ID must be valid.", nameof(skillId));
            }

            if (experience < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(experience));
            }

            if (lastPracticedTotalHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lastPracticedTotalHours));
            }

            SkillId = skillId;
            Experience = experience;
            LastPracticedTotalHours = lastPracticedTotalHours;
        }

        public StableId SkillId { get; }

        public long Experience { get; }

        public long? LastPracticedTotalHours { get; }

        public bool Equals(SkillProgress other)
        {
            return SkillId == other.SkillId &&
                   Experience == other.Experience &&
                   LastPracticedTotalHours == other.LastPracticedTotalHours;
        }

        public override bool Equals(object obj)
        {
            return obj is SkillProgress other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SkillId, Experience, LastPracticedTotalHours);
        }
    }
}
