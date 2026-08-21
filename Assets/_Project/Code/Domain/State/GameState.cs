#nullable enable

using System;
using RapWay.Domain.Activities;
using RapWay.Domain.Character;
using RapWay.Domain.Common;
using RapWay.Domain.Random;
using RapWay.Domain.Time;

namespace RapWay.Domain.State
{
    public sealed class GameState
    {
        public GameState(CalendarState calendar, RandomState random, long revision = 0)
            : this(calendar, random, CharacterState.CreateDefault(), revision)
        {
        }

        public GameState(CalendarState calendar, RandomState random, CharacterState character, long revision = 0)
            : this(calendar, random, character, null, revision)
        {
        }

        public GameState(
            CalendarState calendar,
            RandomState random,
            CharacterState character,
            ActivitySessionState? activeActivity,
            long revision = 0)
        {
            Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Character = character ?? throw new ArgumentNullException(nameof(character));
            ActiveActivity = activeActivity;

            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(revision));
            }

            Revision = revision;
            Validate();
        }

        public CalendarState Calendar { get; }

        public RandomState Random { get; }

        public CharacterState Character { get; }

        public ActivitySessionState? ActiveActivity { get; private set; }

        public long Revision { get; private set; }

        public static GameState Create(GameDate startDate, ulong randomSeed)
        {
            return new GameState(new CalendarState(startDate), new RandomState(randomSeed));
        }

        internal GameState Copy()
        {
            return new GameState(
                Calendar.Copy(),
                Random.Copy(),
                Character.Copy(),
                ActiveActivity?.Copy(),
                Revision);
        }

        internal IRandom GetRandom(StableId streamName)
        {
            return new DeterministicRandom(Random, streamName);
        }

        internal void AdvanceTime(int hours)
        {
            Calendar.Advance(hours);
        }

        internal void BeginActivity(ActivitySessionState activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            if (ActiveActivity != null)
            {
                throw new InvalidOperationException("An activity is already in progress.");
            }

            if (activity.StartedAtTotalHours != Calendar.TotalHours)
            {
                throw new InvalidOperationException("An activity must start at the current game time.");
            }

            ActiveActivity = activity.Copy();
        }

        internal void AdvanceActiveActivity(int hours)
        {
            ActivitySessionState activity = ActiveActivity ??
                                            throw new InvalidOperationException("There is no active activity to advance.");
            if (!Calendar.CanAdvance(hours))
            {
                throw new InvalidOperationException("The calendar cannot advance by the requested activity duration.");
            }

            activity.Advance(hours);
            Calendar.Advance(hours);
        }

        internal void ApplyActiveActivityHour(ActivityHourlyEffects effects)
        {
            if (effects == null)
            {
                throw new ArgumentNullException(nameof(effects));
            }

            AdvanceActiveActivity(1);
            Character.ApplyActivityHour(effects, Calendar.TotalHours);
        }

        internal void CompleteActiveActivity()
        {
            if (ActiveActivity == null)
            {
                throw new InvalidOperationException("There is no active activity to complete.");
            }

            if (!ActiveActivity.IsComplete)
            {
                throw new InvalidOperationException("The active activity is not complete.");
            }

            ActiveActivity = null;
        }

        internal void InterruptActiveActivity()
        {
            if (ActiveActivity == null)
            {
                throw new InvalidOperationException("There is no active activity to interrupt.");
            }

            ActiveActivity = null;
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
            Character.Validate();
            ActiveActivity?.Validate();
        }
    }
}
