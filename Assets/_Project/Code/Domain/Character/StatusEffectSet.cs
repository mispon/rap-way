using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RapWay.Domain.Character
{
    public sealed class StatusEffectSet
    {
        private readonly ReadOnlyCollection<StatusEffectState> _effects;

        public StatusEffectSet(IReadOnlyList<StatusEffectState> effects)
        {
            if (effects == null)
            {
                throw new ArgumentNullException(nameof(effects));
            }

            List<StatusEffectState> copied = new(effects.Count);
            HashSet<string> instanceIds = new(StringComparer.Ordinal);
            for (int index = 0; index < effects.Count; index++)
            {
                StatusEffectState effect = effects[index];
                _ = new StatusEffectState(
                    effect.InstanceId,
                    effect.DefinitionId,
                    effect.SourceId,
                    effect.AppliedAtTotalHours,
                    effect.ExpiresAtTotalHours,
                    effect.Stacks);
                if (!instanceIds.Add(effect.InstanceId.Value))
                {
                    throw new ArgumentException("Status effect instance IDs must be unique.", nameof(effects));
                }

                copied.Add(effect);
            }

            copied.Sort((left, right) => left.InstanceId.CompareTo(right.InstanceId));
            _effects = new ReadOnlyCollection<StatusEffectState>(copied);
        }

        public IReadOnlyList<StatusEffectState> Effects => _effects;

        public static StatusEffectSet Empty { get; } = new(Array.Empty<StatusEffectState>());

        internal StatusEffectSet Copy()
        {
            return new StatusEffectSet(_effects);
        }
    }
}
