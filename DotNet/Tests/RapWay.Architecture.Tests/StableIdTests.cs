using System;
using NUnit.Framework;
using RapWay.Domain.Common;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class StableIdTests
    {
        [TestCase("world.events")]
        [TestCase("npc-decisions")]
        [TestCase("release_reception2")]
        public void CreateAcceptsSemanticAsciiIds(string value)
        {
            StableId stableId = StableId.Create(value);

            Assert.That(stableId.Value, Is.EqualTo(value));
        }

        [TestCase("")]
        [TestCase("World.Events")]
        [TestCase(" world.events")]
        [TestCase("world/events")]
        [TestCase(".world")]
        public void CreateRejectsUnstableIds(string value)
        {
            Assert.That((Action)(() => StableId.Create(value)), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void DefaultValueIsExplicitlyInvalid()
        {
            Assert.That(default(StableId).IsValid, Is.False);
        }
    }
}
