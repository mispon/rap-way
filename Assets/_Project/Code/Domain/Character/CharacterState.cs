using System;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Numerics;

namespace RapWay.Domain.Character
{
    public sealed class CharacterState
    {
        public CharacterState(
            CharacterIdentity identity,
            CharacterResources resources,
            Money wallet,
            AudienceState audience,
            HypeState hype,
            SkillBook skills,
            TalentSet talents,
            StatusEffectSet statusEffects)
        {
            Identity = identity;
            Resources = resources ?? throw new ArgumentNullException(nameof(resources));
            Wallet = wallet;
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Hype = hype ?? throw new ArgumentNullException(nameof(hype));
            Skills = skills ?? throw new ArgumentNullException(nameof(skills));
            Talents = talents ?? throw new ArgumentNullException(nameof(talents));
            StatusEffects = statusEffects ?? throw new ArgumentNullException(nameof(statusEffects));
            Validate();
        }

        public CharacterIdentity Identity { get; }

        public CharacterResources Resources { get; private set; }

        public Money Wallet { get; private set; }

        public AudienceState Audience { get; }

        public HypeState Hype { get; }

        public SkillBook Skills { get; private set; }

        public TalentSet Talents { get; }

        public StatusEffectSet StatusEffects { get; }

        public static CharacterState CreateDefault()
        {
            return new CharacterState(
                new CharacterIdentity(StableId.Create("player"), StableId.Create("start.default")),
                CharacterResources.CreateFull(),
                Money.Zero,
                AudienceState.Empty,
                HypeState.Empty,
                SkillBook.Empty,
                TalentSet.Empty,
                StatusEffectSet.Empty);
        }

        internal CharacterState Copy()
        {
            return new CharacterState(
                Identity,
                Resources.Copy(),
                Wallet,
                Audience.Copy(),
                Hype.Copy(),
                Skills.Copy(),
                Talents.Copy(),
                StatusEffects.Copy());
        }

        internal void ApplyActivityHour(ActivityHourlyEffects effects, long practicedAtTotalHours)
        {
            if (effects == null)
            {
                throw new ArgumentNullException(nameof(effects));
            }

            Resources = Resources.ApplyChanges(
                effects.EnergyChange,
                effects.SatietyChange,
                effects.MotivationChange);

            SkillBook updatedSkills = Skills;
            for (int index = 0; index < effects.SkillExperience.Count; index++)
            {
                ActivitySkillExperienceGrant grant = effects.SkillExperience[index];
                updatedSkills = updatedSkills.GainExperience(
                    grant.SkillId,
                    grant.ExperiencePerHour,
                    practicedAtTotalHours);
            }

            Skills = updatedSkills;
        }

        internal void Credit(Money amount)
        {
            if (amount.MinorUnits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            Wallet = Wallet.Add(amount);
        }

        internal void Validate()
        {
            _ = new CharacterIdentity(Identity.Id, Identity.StartTemplateId);
            Resources.Validate();
            Hype.Validate();
        }
    }
}
