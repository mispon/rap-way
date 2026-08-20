using RapWay.Domain.Common;

namespace RapWay.Domain.Random
{
    public sealed class RandomStreamState
    {
        internal RandomStreamState(StableId name, int algorithmVersion, ulong state)
        {
            Name = name;
            AlgorithmVersion = algorithmVersion;
            State = state;
        }

        public StableId Name { get; }

        public int AlgorithmVersion { get; }

        public ulong State { get; internal set; }

        internal RandomStreamState Copy()
        {
            return new RandomStreamState(Name, AlgorithmVersion, State);
        }
    }
}
