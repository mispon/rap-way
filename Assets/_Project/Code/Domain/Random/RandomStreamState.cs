using System;
using RapWay.Domain.Common;

namespace RapWay.Domain.Random
{
    public sealed class RandomStreamState
    {
        internal RandomStreamState(StableId name, int algorithmVersion, ulong state)
        {
            if (!name.IsValid)
            {
                throw new ArgumentException("A random stream requires a valid stable name.", nameof(name));
            }

            if (algorithmVersion != DeterministicRandom.AlgorithmVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(algorithmVersion));
            }

            if (state == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(state));
            }

            Name = name;
            AlgorithmVersion = algorithmVersion;
            State = state;
        }

        public StableId Name { get; }

        public int AlgorithmVersion { get; }

        public ulong State { get; internal set; }

        public static RandomStreamState Restore(StableId name, int algorithmVersion, ulong state)
        {
            return new RandomStreamState(name, algorithmVersion, state);
        }

        internal RandomStreamState Copy()
        {
            return new RandomStreamState(Name, AlgorithmVersion, State);
        }
    }
}
