using NUnit.Framework;
using RapWay.Application.Events;
using RapWay.Application.Simulation;
using RapWay.Application.Time;
using RapWay.Domain.Events;
using RapWay.Domain.State;
using RapWay.Domain.Time;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class AdvanceTimeCommandTests
    {
        [Test]
        public void SuccessfulCommandAdvancesTimeAndEmitsCommittedFact()
        {
            SimulationSession session = CreateSession();

            var result = session.Execute(new AdvanceTimeCommand(26), new AdvanceTimeCommandHandler());

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.StateRevision, Is.EqualTo(1));
            Assert.That(session.GetStateSnapshot().Calendar.TotalHours, Is.EqualTo(26));
            Assert.That(result.CommittedEvents, Has.Count.EqualTo(1));

            TimeAdvanced timeAdvanced = (TimeAdvanced)result.CommittedEvents[0];
            Assert.That(timeAdvanced.PreviousDate, Is.EqualTo(new GameDate(2026, 1, 1, 8)));
            Assert.That(timeAdvanced.CurrentDate, Is.EqualTo(new GameDate(2026, 1, 2, 10)));
            Assert.That(timeAdvanced.ElapsedHours, Is.EqualTo(26));
        }

        [TestCase(0)]
        [TestCase(-3)]
        public void InvalidDurationIsRejectedWithoutChangingState(int hours)
        {
            SimulationSession session = CreateSession();

            var result = session.Execute(new AdvanceTimeCommand(hours), new AdvanceTimeCommandHandler());

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(TimeCommandFailures.InvalidDuration));
            Assert.That(result.CommittedEvents, Is.Empty);
            Assert.That(session.GetStateSnapshot().Revision, Is.Zero);
            Assert.That(session.GetStateSnapshot().Calendar.TotalHours, Is.Zero);
        }

        private static SimulationSession CreateSession()
        {
            GameState state = GameState.Create(new GameDate(2026, 1, 1, 8), 100);
            return new SimulationSession(state, DiscardCommittedEventSink.Instance);
        }
    }
}
