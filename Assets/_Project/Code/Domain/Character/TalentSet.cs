using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RapWay.Domain.Common;

namespace RapWay.Domain.Character
{
    public sealed class TalentSet
    {
        private readonly ReadOnlyCollection<StableId> _ids;

        public TalentSet(IReadOnlyList<StableId> ids)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            List<StableId> copied = new(ids.Count);
            HashSet<string> values = new(StringComparer.Ordinal);
            for (int index = 0; index < ids.Count; index++)
            {
                StableId id = ids[index];
                if (!id.IsValid || !values.Add(id.Value))
                {
                    throw new ArgumentException("Talent IDs must be valid and unique.", nameof(ids));
                }

                copied.Add(id);
            }

            copied.Sort();
            _ids = new ReadOnlyCollection<StableId>(copied);
        }

        public IReadOnlyList<StableId> Ids => _ids;

        public static TalentSet Empty { get; } = new(Array.Empty<StableId>());

        internal TalentSet Copy()
        {
            return new TalentSet(_ids);
        }
    }
}
