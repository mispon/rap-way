using NUnit.Framework;
using RapWay.Domain.Common;
using RapWay.Domain.Random;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class DeterministicRandomTests
    {
        private static readonly StableId WorldEvents = StableId.Create("world.events");
        private static readonly StableId NpcDecisions = StableId.Create("npc.decisions");

        [Test]
        public void SameSeedAndStreamProduceSameSequence()
        {
            DeterministicRandom first = new(new RandomState(123456), WorldEvents);
            DeterministicRandom second = new(new RandomState(123456), WorldEvents);

            for (int index = 0; index < 100; index++)
            {
                Assert.That(first.NextInt(-50, 50), Is.EqualTo(second.NextInt(-50, 50)));
            }
        }

        [Test]
        public void NamedStreamsAreIndependentOfConsumptionOrder()
        {
            RandomState firstState = new(42);
            DeterministicRandom consumedFirst = new(firstState, WorldEvents);
            _ = consumedFirst.NextInt(0, 1000);
            DeterministicRandom firstNpc = new(firstState, NpcDecisions);

            RandomState secondState = new(42);
            DeterministicRandom secondNpc = new(secondState, NpcDecisions);

            Assert.That(firstNpc.NextInt(0, 1000), Is.EqualTo(secondNpc.NextInt(0, 1000)));
        }

        [Test]
        public void DifferentSeedsProduceDifferentSequences()
        {
            DeterministicRandom first = new(new RandomState(1), WorldEvents);
            DeterministicRandom second = new(new RandomState(2), WorldEvents);

            Assert.That(first.NextInt(0, int.MaxValue), Is.Not.EqualTo(second.NextInt(0, int.MaxValue)));
        }

        [Test]
        public void AlgorithmVersionOneMatchesGoldenSequence()
        {
            DeterministicRandom random = new(new RandomState(123456), WorldEvents);
            int[] expected = { 93, 90, 40, 0, 94 };

            for (int index = 0; index < expected.Length; index++)
            {
                Assert.That(random.NextInt(0, 100), Is.EqualTo(expected[index]));
            }
        }

        [Test]
        public void ChanceHandlesCertainOutcomesWithoutInvalidRanges()
        {
            DeterministicRandom random = new(new RandomState(10), WorldEvents);

            Assert.That(random.Chance(0, 100), Is.False);
            Assert.That(random.Chance(100, 100), Is.True);
        }

        [Test]
        public void DefaultStreamNameIsRejected()
        {
            Assert.That(
                (System.Action)(() => new DeterministicRandom(new RandomState(10), default)),
                Throws.TypeOf<System.ArgumentException>());
        }
    }
}
