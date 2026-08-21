using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Activities
{
    public readonly struct ActivitySkillExperienceGrant
    {
        public const int MaximumExperiencePerHour = 100_000;

        public ActivitySkillExperienceGrant(StableId skillId, int experiencePerHour)
        {
            if (!skillId.IsValid)
            {
                throw new ArgumentException("Skill ID must be valid.", nameof(skillId));
            }

            if (experiencePerHour <= 0 || experiencePerHour > MaximumExperiencePerHour)
            {
                throw new ArgumentOutOfRangeException(nameof(experiencePerHour));
            }

            SkillId = skillId;
            ExperiencePerHour = experiencePerHour;
        }

        public StableId SkillId { get; }

        public int ExperiencePerHour { get; }
    }
}
