using NUnit.Framework;
using RapWay.Application.Activities;
using RapWay.Application.Events;
using RapWay.Application.Simulation;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Events;
using RapWay.Domain.Localization;
using RapWay.Domain.Numerics;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using RapWay.Infrastructure.Activities;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class ActivityCommandTests
    {
        [Test]
        public void CompletedActivityAppliesHourlyEffectsAndSettlesPaymentOnce()
        {
            SimulationSession session = CreateSession();
            IActivityDefinitionLookup definitions = CreateDefinitions(canInterrupt: true);

            var started = session.Execute(
                new StartActivityCommand(StableId.Create("work.courier"), 2),
                new StartActivityCommandHandler(definitions));
            var firstHour = session.Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(definitions));
            var secondHour = session.Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(definitions));
            var completed = session.Execute(
                new CompleteActivityCommand(),
                new CompleteActivityCommandHandler(definitions));

            GameState state = session.GetStateSnapshot();
            Assert.That(started.IsSuccess, Is.True);
            Assert.That(started.StateRevision, Is.EqualTo(1));
            Assert.That(((ActivityStarted)started.CommittedEvents[0]).InstanceId, Is.EqualTo(StableId.Create("activity.1")));
            Assert.That(firstHour.IsSuccess, Is.True);
            Assert.That(secondHour.IsSuccess, Is.True);
            Assert.That(state.Calendar.TotalHours, Is.EqualTo(2));
            Assert.That(state.Character.Resources.Energy.Current, Is.EqualTo(800));
            Assert.That(state.Character.Resources.Satiety.Current, Is.EqualTo(900));
            Assert.That(state.Character.Resources.Motivation.Current, Is.EqualTo(1_000));
            Assert.That(state.Character.Skills.Entries, Has.Count.EqualTo(1));
            Assert.That(state.Character.Skills.Entries[0].Experience, Is.EqualTo(8));
            Assert.That(state.Character.Skills.Entries[0].LastPracticedTotalHours, Is.EqualTo(2));
            Assert.That(state.Character.Wallet.MinorUnits, Is.EqualTo(5_000));
            Assert.That(state.ActiveActivity, Is.Null);
            Assert.That(completed.IsSuccess, Is.True);
            Assert.That(completed.StateRevision, Is.EqualTo(4));
            Assert.That(((ActivityCompleted)completed.CommittedEvents[0]).Payment.MinorUnits, Is.EqualTo(5_000));
        }

        [Test]
        public void InterruptedActivityPaysOnlyForHoursAlreadyWorked()
        {
            SimulationSession session = CreateSession();
            IActivityDefinitionLookup definitions = CreateDefinitions(canInterrupt: true);

            session.Execute(
                new StartActivityCommand(StableId.Create("work.courier"), 4),
                new StartActivityCommandHandler(definitions));
            session.Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(definitions));
            session.Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(definitions));
            var interrupted = session.Execute(
                new InterruptActivityCommand(),
                new InterruptActivityCommandHandler(definitions));

            GameState state = session.GetStateSnapshot();
            Assert.That(interrupted.IsSuccess, Is.True);
            Assert.That(((ActivityInterrupted)interrupted.CommittedEvents[0]).CompletedHours, Is.EqualTo(2));
            Assert.That(((ActivityInterrupted)interrupted.CommittedEvents[0]).Payment.MinorUnits, Is.EqualTo(5_000));
            Assert.That(state.Character.Wallet.MinorUnits, Is.EqualTo(5_000));
            Assert.That(state.ActiveActivity, Is.Null);
        }

        [Test]
        public void NonInterruptibleActivityRejectsInterruptionWithoutChangingState()
        {
            SimulationSession session = CreateSession();
            IActivityDefinitionLookup definitions = CreateDefinitions(canInterrupt: false);

            session.Execute(
                new StartActivityCommand(StableId.Create("work.courier"), 4),
                new StartActivityCommandHandler(definitions));
            session.Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(definitions));

            var result = session.Execute(
                new InterruptActivityCommand(),
                new InterruptActivityCommandHandler(definitions));

            GameState state = session.GetStateSnapshot();
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(ActivityCommandFailures.CannotInterrupt));
            Assert.That(result.CommittedEvents, Is.Empty);
            Assert.That(state.Revision, Is.EqualTo(2));
            Assert.That(state.ActiveActivity, Is.Not.Null);
            Assert.That(state.ActiveActivity!.ElapsedHours, Is.EqualTo(1));
            Assert.That(state.Character.Wallet, Is.EqualTo(Money.Zero));
        }

        [Test]
        public void ActivityCannotStartWithUnsupportedDuration()
        {
            SimulationSession session = CreateSession();
            IActivityDefinitionLookup definitions = CreateDefinitions(canInterrupt: true);

            var result = session.Execute(
                new StartActivityCommand(StableId.Create("work.courier"), 1),
                new StartActivityCommandHandler(definitions));

            GameState state = session.GetStateSnapshot();
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(ActivityCommandFailures.InvalidDuration));
            Assert.That(state.Revision, Is.Zero);
            Assert.That(state.ActiveActivity, Is.Null);
            Assert.That(state.Calendar.TotalHours, Is.Zero);
        }

        private static SimulationSession CreateSession()
        {
            return new SimulationSession(
                GameState.Create(new GameDate(2026, 1, 1, 8), 100),
                DiscardCommittedEventSink.Instance);
        }

        private static IActivityDefinitionLookup CreateDefinitions(bool canInterrupt)
        {
            ActivityDefinition definition = new(
                StableId.Create("work.courier"),
                ActivityCategory.Work,
                2,
                8,
                new LocalizationKey("Activities", "work_courier_title"),
                new LocalizationKey("Activities", "work_courier_description"),
                new ActivityHourlyEffects(
                    energyChange: -100,
                    satietyChange: -50,
                    motivationChange: 10,
                    new[] { new ActivitySkillExperienceGrant(StableId.Create("skill.endurance"), 4) }),
                Money.FromMinorUnits(2_500),
                default,
                canInterrupt,
                canAccelerateWhenFamiliar: true);

            return new ActivityDefinitionCatalog(new[] { definition });
        }
    }
}
