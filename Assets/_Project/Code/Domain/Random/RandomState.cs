using System;
using System.Collections.Generic;
using RapWay.Domain.Common;

namespace RapWay.Domain.Random
{
    public sealed class RandomState
    {
        private readonly List<RandomStreamState> _streams;

        public RandomState(ulong masterSeed)
            : this(masterSeed, new List<RandomStreamState>())
        {
        }

        private RandomState(ulong masterSeed, List<RandomStreamState> streams)
        {
            MasterSeed = masterSeed;
            _streams = streams;
            Validate();
        }

        public ulong MasterSeed { get; }

        public int StreamCount => _streams.Count;

        public static RandomState Restore(ulong masterSeed, IReadOnlyList<RandomStreamState> streams)
        {
            if (streams == null)
            {
                throw new ArgumentNullException(nameof(streams));
            }

            List<RandomStreamState> copies = new(streams.Count);
            for (int index = 0; index < streams.Count; index++)
            {
                RandomStreamState stream = streams[index] ??
                                           throw new ArgumentException("Random streams cannot contain null.", nameof(streams));
                copies.Add(stream.Copy());
            }

            return new RandomState(masterSeed, copies);
        }

        public IReadOnlyList<RandomStreamState> CaptureStreams()
        {
            RandomStreamState[] copies = new RandomStreamState[_streams.Count];
            for (int index = 0; index < _streams.Count; index++)
            {
                copies[index] = _streams[index].Copy();
            }

            return Array.AsReadOnly(copies);
        }

        internal RandomStreamState GetOrCreateStream(StableId name)
        {
            if (!name.IsValid)
            {
                throw new ArgumentException("A random stream requires a valid stable name.", nameof(name));
            }

            for (int index = 0; index < _streams.Count; index++)
            {
                RandomStreamState stream = _streams[index];
                if (stream.Name == name)
                {
                    return stream;
                }
            }

            ulong initialState = DeriveInitialState(MasterSeed, name);
            RandomStreamState created = new(name, DeterministicRandom.AlgorithmVersion, initialState);
            _streams.Add(created);
            return created;
        }

        internal RandomState Copy()
        {
            List<RandomStreamState> streams = new(_streams.Count);
            for (int index = 0; index < _streams.Count; index++)
            {
                streams.Add(_streams[index].Copy());
            }

            return new RandomState(MasterSeed, streams);
        }

        internal void Validate()
        {
            HashSet<StableId> names = new();
            for (int index = 0; index < _streams.Count; index++)
            {
                RandomStreamState stream = _streams[index];
                if (!names.Add(stream.Name))
                {
                    throw new InvalidOperationException($"Duplicate random stream '{stream.Name}'.");
                }

                if (stream.AlgorithmVersion != DeterministicRandom.AlgorithmVersion || stream.State == 0)
                {
                    throw new InvalidOperationException($"Random stream '{stream.Name}' is not compatible with the current algorithm.");
                }
            }
        }

        private static ulong DeriveInitialState(ulong masterSeed, StableId name)
        {
            const ulong offsetBasis = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;

            ulong hash = offsetBasis;
            string value = name.Value;
            for (int index = 0; index < value.Length; index++)
            {
                hash ^= value[index];
                hash *= prime;
            }

            ulong mixed = masterSeed ^ hash ^ 0x9E3779B97F4A7C15UL;
            mixed ^= mixed >> 30;
            mixed *= 0xBF58476D1CE4E5B9UL;
            mixed ^= mixed >> 27;
            mixed *= 0x94D049BB133111EBUL;
            mixed ^= mixed >> 31;
            return mixed == 0 ? 0xA0761D6478BD642FUL : mixed;
        }
    }
}
