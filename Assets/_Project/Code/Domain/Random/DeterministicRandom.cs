using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Random
{
    public sealed class DeterministicRandom : IRandom
    {
        public const int AlgorithmVersion = 1;

        private readonly RandomStreamState _stream;

        public DeterministicRandom(RandomState state, StableId streamName)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _stream = state.GetOrCreateStream(streamName);
        }

        public int NextInt(int minimumInclusive, int maximumExclusive)
        {
            if (minimumInclusive >= maximumExclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumExclusive));
            }

            ulong range = (ulong)((long)maximumExclusive - minimumInclusive);
            ulong threshold = unchecked(0UL - range) % range;
            ulong sample;

            do
            {
                sample = NextUInt64();
            }
            while (sample < threshold);

            long offset = (long)(sample % range);
            return checked((int)(minimumInclusive + offset));
        }

        public bool Chance(int successfulWeight, int totalWeight)
        {
            if (totalWeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalWeight));
            }

            if (successfulWeight < 0 || successfulWeight > totalWeight)
            {
                throw new ArgumentOutOfRangeException(nameof(successfulWeight));
            }

            return successfulWeight != 0 &&
                   (successfulWeight == totalWeight || NextInt(0, totalWeight) < successfulWeight);
        }

        private ulong NextUInt64()
        {
            ulong value = _stream.State;
            value ^= value >> 12;
            value ^= value << 25;
            value ^= value >> 27;
            _stream.State = value;
            return value * 2685821657736338717UL;
        }
    }
}
