using System;
using System.Collections.Generic;
using NUnit.Framework;
using RapWay.Application.Commands;
using RapWay.Application.Events;
using RapWay.Application.Simulation;
using RapWay.Domain.Common;
using RapWay.Domain.Events;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class SimulationSessionTests
    {
        private static readonly StableId TestStream = StableId.Create("test.stream");
        private static readonly CommandFailure RejectedFailure =
            new(StableId.Create("test.rejected"));

        [Test]
        public void RejectedCommandDiscardsAllWorkingStateMutations()
        {
            RecordingEventSink sink = new();
            SimulationSession session = CreateSession(sink);

            CommandResult result = session.Execute(new TestCommand(), new MutateThenRejectHandler());

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(RejectedFailure));
            Assert.That(session.GetStateSnapshot().Revision, Is.Zero);
            Assert.That(session.GetStateSnapshot().Random.StreamCount, Is.Zero);
            Assert.That(sink.Events, Is.Empty);
        }

        [Test]
        public void HandlerExceptionDiscardsAllWorkingStateMutations()
        {
            RecordingEventSink sink = new();
            SimulationSession session = CreateSession(sink);

            Assert.That(
                (Action)(() => session.Execute(new TestCommand(), new MutateThenThrowHandler())),
                Throws.TypeOf<InvalidOperationException>());

            Assert.That(session.GetStateSnapshot().Revision, Is.Zero);
            Assert.That(session.GetStateSnapshot().Random.StreamCount, Is.Zero);
            Assert.That(sink.Events, Is.Empty);
        }

        [Test]
        public void EventsArePublishedInHandlerOrderAfterCommit()
        {
            SimulationSession? session = null;
            long revisionObservedBySink = -1;
            RecordingEventSink sink = new(events =>
            {
                revisionObservedBySink = session!.GetStateSnapshot().Revision;
            });
            session = CreateSession(sink);

            CommandResult result = session.Execute(new TestCommand(), new OrderedEventsHandler());

            Assert.That(revisionObservedBySink, Is.EqualTo(1));
            Assert.That(result.StateRevision, Is.EqualTo(1));
            Assert.That(sink.Events, Has.Count.EqualTo(2));
            Assert.That(((TestEvent)sink.Events[0]).Order, Is.EqualTo(1));
            Assert.That(((TestEvent)sink.Events[1]).Order, Is.EqualTo(2));
        }

        [Test]
        public void PublisherFailureCannotRollbackCommittedState()
        {
            SimulationSession session = CreateSession(new ThrowingEventSink());

            Assert.That(
                (Action)(() => session.Execute(new TestCommand(), new OrderedEventsHandler())),
                Throws.TypeOf<EventPublicationException>());

            Assert.That(session.GetStateSnapshot().Revision, Is.EqualTo(1));
        }

        [Test]
        public void ReturnedSnapshotCannotMutateAuthoritativeRandomState()
        {
            SimulationSession session = CreateSession(DiscardCommittedEventSink.Instance);
            GameState snapshot = session.GetStateSnapshot();
            DeterministicRandom snapshotRandom = new(snapshot.Random, TestStream);

            _ = snapshotRandom.NextInt(0, 100);

            Assert.That(session.GetStateSnapshot().Random.StreamCount, Is.Zero);
        }

        [Test]
        public void HandlerCannotMutateCommittedStateThroughRetainedWorkingReference()
        {
            SimulationSession session = CreateSession(DiscardCommittedEventSink.Instance);
            RetainingHandler handler = new();

            _ = session.Execute(new TestCommand(), handler);
            handler.MutateRetainedState();

            Assert.That(session.GetStateSnapshot().Random.StreamCount, Is.Zero);
            Assert.That(session.GetStateSnapshot().Revision, Is.EqualTo(1));
        }

        private static SimulationSession CreateSession(ICommittedEventSink sink)
        {
            return new SimulationSession(
                GameState.Create(new GameDate(2026, 1, 1, 0), 55),
                sink);
        }

        private sealed class TestCommand : IGameCommand
        {
        }

        private sealed class TestEvent : IDomainEvent
        {
            public TestEvent(int order)
            {
                Order = order;
            }

            public int Order { get; }
        }

        private sealed class MutateThenRejectHandler : ICommandHandler<TestCommand>
        {
            public CommandExecution Execute(GameState state, TestCommand command)
            {
                DeterministicRandom random = new(state.Random, TestStream);
                _ = random.NextInt(0, 100);
                return CommandExecution.Rejected(RejectedFailure);
            }
        }

        private sealed class MutateThenThrowHandler : ICommandHandler<TestCommand>
        {
            public CommandExecution Execute(GameState state, TestCommand command)
            {
                DeterministicRandom random = new(state.Random, TestStream);
                _ = random.NextInt(0, 100);
                throw new InvalidOperationException("Expected test failure.");
            }
        }

        private sealed class OrderedEventsHandler : ICommandHandler<TestCommand>
        {
            public CommandExecution Execute(GameState state, TestCommand command)
            {
                return CommandExecution.Succeeded(new TestEvent(1), new TestEvent(2));
            }
        }

        private sealed class RetainingHandler : ICommandHandler<TestCommand>
        {
            private GameState? _retainedState;

            public CommandExecution Execute(GameState state, TestCommand command)
            {
                _retainedState = state;
                return CommandExecution.Succeeded();
            }

            public void MutateRetainedState()
            {
                DeterministicRandom random = new(_retainedState!.Random, TestStream);
                _ = random.NextInt(0, 100);
            }
        }

        private sealed class RecordingEventSink : ICommittedEventSink
        {
            private readonly Action<IReadOnlyList<IDomainEvent>>? _onPublish;

            public RecordingEventSink(Action<IReadOnlyList<IDomainEvent>>? onPublish = null)
            {
                _onPublish = onPublish;
            }

            public List<IDomainEvent> Events { get; } = new();

            public void Publish(IReadOnlyList<IDomainEvent> events)
            {
                _onPublish?.Invoke(events);
                Events.AddRange(events);
            }
        }

        private sealed class ThrowingEventSink : ICommittedEventSink
        {
            public void Publish(IReadOnlyList<IDomainEvent> events)
            {
                throw new EventPublicationException();
            }
        }

        private sealed class EventPublicationException : Exception
        {
        }
    }
}
