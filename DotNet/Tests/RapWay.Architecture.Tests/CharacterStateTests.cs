using System;
using NUnit.Framework;
using RapWay.Domain.Character;
using RapWay.Domain.Common;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class CharacterStateTests
    {
        [Test]
        public void ResourcesClampAtTheirExplicitBounds()
        {
            BoundedResource resource = new(800, 1_000);

            BoundedResource gained = resource.Gain(500);
            BoundedResource spent = resource.Spend(2_000);

            Assert.That(gained.Current, Is.EqualTo(1_000));
            Assert.That(spent.Current, Is.Zero);
        }

        [Test]
        public void SkillBookSortsEntriesAndRejectsDuplicates()
        {
            StableId lyrics = StableId.Create("skill.lyrics");
            StableId beatmaking = StableId.Create("skill.beatmaking");
            SkillBook skills = new(new[]
            {
                new SkillProgress(lyrics, 100, 24),
                new SkillProgress(beatmaking, 80, null)
            });

            Assert.That(skills.Entries[0].SkillId, Is.EqualTo(beatmaking));
            Assert.That(skills.Entries[1].LastPracticedTotalHours, Is.EqualTo(24));
            Assert.That(
                (Action)(() => new SkillBook(new[]
                {
                    new SkillProgress(lyrics, 1, null),
                    new SkillProgress(lyrics, 2, null)
                })),
                Throws.ArgumentException);
        }

        [Test]
        public void StatusEffectRejectsAnExpiryBeforeItsApplication()
        {
            Assert.That(
                (Action)(() => new StatusEffectState(
                    StableId.Create("effect.1"),
                    StableId.Create("status.insomnia"),
                    StableId.Create("event.bad_night"),
                    12,
                    11,
                    1)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void DefaultCharacterHasAPlayableNeutralState()
        {
            CharacterState character = CharacterState.CreateDefault();

            Assert.That(character.Identity.Id, Is.EqualTo(StableId.Create("player")));
            Assert.That(character.Resources.Energy.Current, Is.EqualTo(CharacterResources.DefaultMaximum));
            Assert.That(character.Resources.Satiety.Current, Is.EqualTo(CharacterResources.DefaultMaximum));
            Assert.That(character.Resources.Motivation.Current, Is.EqualTo(CharacterResources.DefaultMaximum));
            Assert.That(character.Wallet.MinorUnits, Is.Zero);
            Assert.That(character.Audience.TotalFans, Is.Zero);
            Assert.That(character.Hype.Intensity.Current, Is.Zero);
        }
    }
}
