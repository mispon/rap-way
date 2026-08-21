using System;
using NUnit.Framework;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class ActivitySessionStateTests
    {
        [Test]
        public void ActiveActivityAdvancesWithTheCalendarAndCompletesExplicitly()
        {
            GameState state = GameState.Create(new GameDate(2026, 1, 1, 8), 5);
            ActivitySessionState session = new(
                StableId.Create("activity.1"),
                StableId.Create("work.courier"),
                state.Calendar.TotalHours,
                4);

            state.BeginActivity(session);
            state.AdvanceActiveActivity(2);

            Assert.That(state.Calendar.TotalHours, Is.EqualTo(2));
            Assert.That(state.ActiveActivity, Is.Not.Null);
            Assert.That(state.ActiveActivity!.ElapsedHours, Is.EqualTo(2));
            Assert.That(state.ActiveActivity.IsComplete, Is.False);
            Assert.That(
                (Action)(state.CompleteActiveActivity),
                Throws.TypeOf<InvalidOperationException>());

            state.AdvanceActiveActivity(2);
            state.CompleteActiveActivity();

            Assert.That(state.Calendar.TotalHours, Is.EqualTo(4));
            Assert.That(state.ActiveActivity, Is.Null);
        }

        [Test]
        public void ActivityRequiresTheCurrentGameTimeAndRejectsOverlap()
        {
            GameState state = GameState.Create(new GameDate(2026, 1, 1, 8), 5);
            ActivitySessionState staleSession = new(
                StableId.Create("activity.stale"),
                StableId.Create("work.courier"),
                1,
                2);

            Assert.That(
                (Action)(() => state.BeginActivity(staleSession)),
                Throws.TypeOf<InvalidOperationException>());

            ActivitySessionState first = new(
                StableId.Create("activity.first"),
                StableId.Create("work.courier"),
                0,
                2);
            state.BeginActivity(first);

            Assert.That(
                (Action)(() => state.BeginActivity(new ActivitySessionState(
                    StableId.Create("activity.second"),
                    StableId.Create("rest.sleep"),
                    0,
                    2))),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void ActivityCannotAdvancePastItsRemainingDuration()
        {
            ActivitySessionState session = new(
                StableId.Create("activity.1"),
                StableId.Create("work.courier"),
                0,
                2);

            Assert.That(
                (Action)(() => session.Advance(3)),
                Throws.TypeOf<InvalidOperationException>());
        }
    }
}
