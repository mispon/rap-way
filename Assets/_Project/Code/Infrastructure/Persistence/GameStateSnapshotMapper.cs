#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using RapWay.Domain.Activities;
using RapWay.Domain.Character;
using RapWay.Domain.Common;
using RapWay.Domain.Numerics;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using RapWay.Infrastructure.Persistence.Dto;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class GameStateSnapshotMapper
    {
        public GameSaveDto Capture(GameState state, DateTime savedAtUtc)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (savedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Save timestamps must be UTC.", nameof(savedAtUtc));
            }

            IReadOnlyList<RandomStreamState> capturedStreams = state.Random.CaptureStreams();
            List<RandomStreamState> sortedStreams = new(capturedStreams);
            sortedStreams.Sort((left, right) => left.Name.CompareTo(right.Name));

            List<RandomStreamStateDto> streamDtos = new(sortedStreams.Count);
            for (int index = 0; index < sortedStreams.Count; index++)
            {
                RandomStreamState stream = sortedStreams[index];
                streamDtos.Add(new RandomStreamStateDto
                {
                    Name = stream.Name.Value,
                    AlgorithmVersion = stream.AlgorithmVersion,
                    State = stream.State.ToString(CultureInfo.InvariantCulture)
                });
            }

            GameDate startDate = state.Calendar.StartDate;
            return new GameSaveDto
            {
                SavedAtUtc = savedAtUtc.ToString("O", CultureInfo.InvariantCulture),
                State = new GameStateDto
                {
                    Revision = state.Revision,
                    Calendar = new CalendarStateDto
                    {
                        StartYear = startDate.Year,
                        StartMonth = startDate.Month,
                        StartDay = startDate.Day,
                        StartHour = startDate.Hour,
                        TotalHours = state.Calendar.TotalHours
                    },
                    Random = new RandomStateDto
                    {
                        MasterSeed = state.Random.MasterSeed.ToString(CultureInfo.InvariantCulture),
                        Streams = streamDtos
                    },
                    Character = CaptureCharacter(state.Character),
                    ActiveActivity = CaptureActivity(state.ActiveActivity)
                }
            };
        }

        public DecodedGameSave Restore(GameSaveDto save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            if (!DateTime.TryParseExact(
                    save.SavedAtUtc,
                    "O",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime savedAtUtc) || savedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new FormatException("The save timestamp must be an ISO-8601 UTC value.");
            }

            if (!ulong.TryParse(save.State.Random.MasterSeed, NumberStyles.None, CultureInfo.InvariantCulture, out ulong masterSeed))
            {
                throw new FormatException("The random master seed is invalid.");
            }

            List<RandomStreamState> streams = new(save.State.Random.Streams.Count);
            for (int index = 0; index < save.State.Random.Streams.Count; index++)
            {
                RandomStreamStateDto streamDto = save.State.Random.Streams[index] ??
                                                   throw new FormatException("A random stream entry is missing.");
                if (!ulong.TryParse(streamDto.State, NumberStyles.None, CultureInfo.InvariantCulture, out ulong streamState))
                {
                    throw new FormatException($"Random stream '{streamDto.Name}' has an invalid state.");
                }

                streams.Add(RandomStreamState.Restore(
                    StableId.Create(streamDto.Name),
                    streamDto.AlgorithmVersion,
                    streamState));
            }

            CalendarState calendar = new(
                new GameDate(
                    save.State.Calendar.StartYear,
                    save.State.Calendar.StartMonth,
                    save.State.Calendar.StartDay,
                    save.State.Calendar.StartHour),
                save.State.Calendar.TotalHours);
            RandomState random = RandomState.Restore(masterSeed, streams);
            CharacterState character = RestoreCharacter(save.State.Character);
            ActivitySessionState? activeActivity = RestoreActivity(save.State.ActiveActivity);
            return new DecodedGameSave(new GameState(calendar, random, character, activeActivity, save.State.Revision), savedAtUtc);
        }

        private static ActivitySessionDto? CaptureActivity(ActivitySessionState? activity)
        {
            if (activity == null)
            {
                return null;
            }

            return new ActivitySessionDto
            {
                InstanceId = activity.InstanceId.Value,
                DefinitionId = activity.DefinitionId.Value,
                StartedAtTotalHours = activity.StartedAtTotalHours,
                DurationHours = activity.DurationHours,
                ElapsedHours = activity.ElapsedHours
            };
        }

        private static ActivitySessionState? RestoreActivity(ActivitySessionDto? activity)
        {
            if (activity == null)
            {
                return null;
            }

            return new ActivitySessionState(
                StableId.Create(activity.InstanceId),
                StableId.Create(activity.DefinitionId),
                activity.StartedAtTotalHours,
                activity.DurationHours,
                activity.ElapsedHours);
        }

        private static CharacterStateDto CaptureCharacter(CharacterState character)
        {
            List<AudienceSegmentDto> audience = new(character.Audience.Segments.Count);
            for (int index = 0; index < character.Audience.Segments.Count; index++)
            {
                AudienceSegment segment = character.Audience.Segments[index];
                audience.Add(new AudienceSegmentDto
                {
                    GroupId = segment.GroupId.Value,
                    FanCount = segment.FanCount.ToString(CultureInfo.InvariantCulture)
                });
            }

            List<SkillProgressDto> skills = new(character.Skills.Entries.Count);
            for (int index = 0; index < character.Skills.Entries.Count; index++)
            {
                SkillProgress skill = character.Skills.Entries[index];
                skills.Add(new SkillProgressDto
                {
                    SkillId = skill.SkillId.Value,
                    Experience = skill.Experience.ToString(CultureInfo.InvariantCulture),
                    LastPracticedTotalHours = skill.LastPracticedTotalHours
                });
            }

            List<string> talentIds = new(character.Talents.Ids.Count);
            for (int index = 0; index < character.Talents.Ids.Count; index++)
            {
                talentIds.Add(character.Talents.Ids[index].Value);
            }

            List<StatusEffectDto> statusEffects = new(character.StatusEffects.Effects.Count);
            for (int index = 0; index < character.StatusEffects.Effects.Count; index++)
            {
                StatusEffectState effect = character.StatusEffects.Effects[index];
                statusEffects.Add(new StatusEffectDto
                {
                    InstanceId = effect.InstanceId.Value,
                    DefinitionId = effect.DefinitionId.Value,
                    SourceId = effect.SourceId.Value,
                    AppliedAtTotalHours = effect.AppliedAtTotalHours,
                    ExpiresAtTotalHours = effect.ExpiresAtTotalHours,
                    Stacks = effect.Stacks
                });
            }

            return new CharacterStateDto
            {
                Id = character.Identity.Id.Value,
                StartTemplateId = character.Identity.StartTemplateId.Value,
                Energy = CaptureResource(character.Resources.Energy),
                Satiety = CaptureResource(character.Resources.Satiety),
                Motivation = CaptureResource(character.Resources.Motivation),
                WalletMinorUnits = character.Wallet.MinorUnits.ToString(CultureInfo.InvariantCulture),
                Audience = audience,
                Hype = CaptureResource(character.Hype.Intensity),
                Skills = skills,
                TalentIds = talentIds,
                StatusEffects = statusEffects
            };
        }

        private static CharacterState RestoreCharacter(CharacterStateDto character)
        {
            if (character == null)
            {
                throw new FormatException("The character state is missing.");
            }

            List<AudienceSegment> audience = new(character.Audience?.Count ?? 0);
            foreach (AudienceSegmentDto segment in character.Audience ?? throw new FormatException("Character audience is missing."))
            {
                if (segment == null || !long.TryParse(segment.FanCount, NumberStyles.None, CultureInfo.InvariantCulture, out long fanCount))
                {
                    throw new FormatException("Character audience contains an invalid fan count.");
                }

                audience.Add(new AudienceSegment(StableId.Create(segment.GroupId), fanCount));
            }

            List<SkillProgress> skills = new(character.Skills?.Count ?? 0);
            foreach (SkillProgressDto skill in character.Skills ?? throw new FormatException("Character skills are missing."))
            {
                if (skill == null || !long.TryParse(skill.Experience, NumberStyles.None, CultureInfo.InvariantCulture, out long experience))
                {
                    throw new FormatException("Character skills contain invalid experience.");
                }

                skills.Add(new SkillProgress(StableId.Create(skill.SkillId), experience, skill.LastPracticedTotalHours));
            }

            List<StableId> talents = new(character.TalentIds?.Count ?? 0);
            foreach (string talentId in character.TalentIds ?? throw new FormatException("Character talents are missing."))
            {
                talents.Add(StableId.Create(talentId));
            }

            List<StatusEffectState> statusEffects = new(character.StatusEffects?.Count ?? 0);
            foreach (StatusEffectDto effect in character.StatusEffects ?? throw new FormatException("Character status effects are missing."))
            {
                if (effect == null)
                {
                    throw new FormatException("Character status effects contain a missing entry.");
                }

                statusEffects.Add(new StatusEffectState(
                    StableId.Create(effect.InstanceId),
                    StableId.Create(effect.DefinitionId),
                    StableId.Create(effect.SourceId),
                    effect.AppliedAtTotalHours,
                    effect.ExpiresAtTotalHours,
                    effect.Stacks));
            }

            if (!long.TryParse(character.WalletMinorUnits, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long walletMinorUnits))
            {
                throw new FormatException("Character wallet is invalid.");
            }

            return new CharacterState(
                new CharacterIdentity(StableId.Create(character.Id), StableId.Create(character.StartTemplateId)),
                new CharacterResources(
                    RestoreResource(character.Energy, "energy"),
                    RestoreResource(character.Satiety, "satiety"),
                    RestoreResource(character.Motivation, "motivation")),
                Money.FromMinorUnits(walletMinorUnits),
                new AudienceState(audience),
                new HypeState(RestoreResource(character.Hype, "hype")),
                new SkillBook(skills),
                new TalentSet(talents),
                new StatusEffectSet(statusEffects));
        }

        private static BoundedResourceDto CaptureResource(BoundedResource resource)
        {
            return new BoundedResourceDto
            {
                Current = resource.Current,
                Maximum = resource.Maximum
            };
        }

        private static BoundedResource RestoreResource(BoundedResourceDto resource, string name)
        {
            if (resource == null)
            {
                throw new FormatException($"Character {name} is missing.");
            }

            return new BoundedResource(resource.Current, resource.Maximum);
        }
    }
}
