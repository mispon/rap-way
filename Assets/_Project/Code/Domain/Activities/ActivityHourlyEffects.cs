using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RapWay.Domain.Activities
{
    public sealed class ActivityHourlyEffects
    {
        public const int MaximumAbsoluteResourceChange = 1_000;

        private readonly ReadOnlyCollection<ActivitySkillExperienceGrant> _skillExperience;

        public ActivityHourlyEffects(
            int energyChange,
            int satietyChange,
            int motivationChange,
            IReadOnlyList<ActivitySkillExperienceGrant> skillExperience)
        {
            ValidateResourceChange(energyChange, nameof(energyChange));
            ValidateResourceChange(satietyChange, nameof(satietyChange));
            ValidateResourceChange(motivationChange, nameof(motivationChange));
            if (skillExperience == null)
            {
                throw new ArgumentNullException(nameof(skillExperience));
            }

            List<ActivitySkillExperienceGrant> copied = new(skillExperience.Count);
            HashSet<string> skillIds = new(StringComparer.Ordinal);
            for (int index = 0; index < skillExperience.Count; index++)
            {
                ActivitySkillExperienceGrant grant = skillExperience[index];
                _ = new ActivitySkillExperienceGrant(grant.SkillId, grant.ExperiencePerHour);
                if (!skillIds.Add(grant.SkillId.Value))
                {
                    throw new ArgumentException("Activity skill experience grants must have unique skill IDs.", nameof(skillExperience));
                }

                copied.Add(grant);
            }

            copied.Sort((left, right) => left.SkillId.CompareTo(right.SkillId));
            EnergyChange = energyChange;
            SatietyChange = satietyChange;
            MotivationChange = motivationChange;
            _skillExperience = new ReadOnlyCollection<ActivitySkillExperienceGrant>(copied);
        }

        public int EnergyChange { get; }

        public int SatietyChange { get; }

        public int MotivationChange { get; }

        public IReadOnlyList<ActivitySkillExperienceGrant> SkillExperience => _skillExperience;

        private static void ValidateResourceChange(int value, string parameterName)
        {
            if (value < -MaximumAbsoluteResourceChange || value > MaximumAbsoluteResourceChange)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
