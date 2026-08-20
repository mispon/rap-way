using System;
using RapWay.Domain.Common;
using RapWay.Domain.Random;
using RapWay.Domain.Time;

namespace RapWay.Domain.State
{
    public sealed class GameState
    {
        public GameState(CalendarState calendar, RandomState random, long revision = 0)
        {
            Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
            Random = random ?? throw new ArgumentNullException(nameof(random));

            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revision));
            }

            Revision = revision;
            Validate();
        }

        public CalendarState Calendar { get; }

        public RandomState Random { get; }

        public long Revision { get; private set; }

        public static GameState Create(GameDate startDate, ulong randomSeed)
        {
            return new GameState(new CalendarState(startDate), new RandomState(randomSeed));
        }

        internal GameState Copy()
        {
            return new GameState(Calendar.Copy(), Random.Copy(), Revision);
        }

        internal IRandom GetRandom(StableId streamName)
        {
            return new DeterministicRandom(Random, streamName);
        }

        internal void AdvanceTime(int hours)
        {
            Calendar.Advance(hours);
        }

        internal void CommitNextRevision(long expectedRevision)
        {
            if (Revision != expectedRevision)
            {
                throw new InvalidOperationException("The working state revision no longer matches the authoritative state.");
            }

            Revision = checked(Revision + 1);
        }

        internal void Validate()
        {
            if (Revision < 0)
            {
                throw new InvalidOperationException("State revision cannot be negative.");
            }

            Calendar.Validate();
            Random.Validate();
        }
    }
}
