using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RapWay.Domain.Character
{
    public sealed class AudienceState
    {
        private readonly ReadOnlyCollection<AudienceSegment> _segments;

        public AudienceState(IReadOnlyList<AudienceSegment> segments)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            List<AudienceSegment> copied = new(segments.Count);
            HashSet<string> groupIds = new(StringComparer.Ordinal);
            long totalFans = 0;
            for (int index = 0; index < segments.Count; index++)
            {
                AudienceSegment segment = segments[index];
                _ = new AudienceSegment(segment.GroupId, segment.FanCount);
                if (!groupIds.Add(segment.GroupId.Value))
                {
                    throw new ArgumentException("Audience groups must be unique.", nameof(segments));
                }

                totalFans = checked(totalFans + segment.FanCount);
                copied.Add(segment);
            }

            copied.Sort((left, right) => left.GroupId.CompareTo(right.GroupId));
            _segments = new ReadOnlyCollection<AudienceSegment>(copied);
            TotalFans = totalFans;
        }

        public IReadOnlyList<AudienceSegment> Segments => _segments;

        public long TotalFans { get; }

        public static AudienceState Empty { get; } = new(Array.Empty<AudienceSegment>());

        internal AudienceState Copy()
        {
            return new AudienceState(_segments);
        }
    }
}
