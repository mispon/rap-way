using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public sealed class SkillBook
    {
        private readonly ReadOnlyCollection<SkillProgress> _entries;

        public SkillBook(IReadOnlyList<SkillProgress> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            List<SkillProgress> copied = new(entries.Count);
            HashSet<string> skillIds = new(StringComparer.Ordinal);
            for (int index = 0; index < entries.Count; index++)
            {
                SkillProgress entry = entries[index];
                _ = new SkillProgress(entry.SkillId, entry.Experience, entry.LastPracticedTotalHours);
                if (!skillIds.Add(entry.SkillId.Value))
                {
                    throw new ArgumentException("Skill IDs must be unique.", nameof(entries));
                }

                copied.Add(entry);
            }

            copied.Sort((left, right) => left.SkillId.CompareTo(right.SkillId));
            _entries = new ReadOnlyCollection<SkillProgress>(copied);
        }

        public IReadOnlyList<SkillProgress> Entries => _entries;

        public static SkillBook Empty { get; } = new(Array.Empty<SkillProgress>());

        internal SkillBook Copy()
        {
            return new SkillBook(_entries);
        }

        internal SkillBook GainExperience(StableId skillId, int experience, long practicedAtTotalHours)
        {
            if (!skillId.IsValid)
            {
                throw new ArgumentException("Skill ID must be valid.", nameof(skillId));
            }

            if (experience <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(experience));
            }

            if (practicedAtTotalHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(practicedAtTotalHours));
            }

            List<SkillProgress> updated = new(_entries.Count + 1);
            bool found = false;
            for (int index = 0; index < _entries.Count; index++)
            {
                SkillProgress entry = _entries[index];
                if (entry.SkillId != skillId)
                {
                    updated.Add(entry);
                    continue;
                }

                updated.Add(new SkillProgress(
                    entry.SkillId,
                    checked(entry.Experience + experience),
                    practicedAtTotalHours));
                found = true;
            }

            if (!found)
            {
                updated.Add(new SkillProgress(skillId, experience, practicedAtTotalHours));
            }

            return new SkillBook(updated);
        }
    }
}
