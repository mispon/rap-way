using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RapWay.Domain.Activities;
using RapWay.Domain.Character;
using RapWay.Domain.Common;
using RapWay.Domain.Numerics;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using RapWay.Infrastructure.Persistence;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class GameSaveSerializerTests
    {
        private static readonly DateTime SavedAtUtc = new(2026, 8, 20, 12, 30, 0, DateTimeKind.Utc);

        [Test]
        public void RoundTripPreservesCalendarRevisionAndUnsignedRandomValues()
        {
            RandomStreamState stream = RandomStreamState.Restore(
                StableId.Create("world.events"),
                DeterministicRandom.AlgorithmVersion,
                ulong.MaxValue);
            GameState state = new(
                new CalendarState(new GameDate(1, 1, 1, 0), CalendarState.MaximumTotalHours),
                RandomState.Restore(ulong.MaxValue, new[] { stream }),
                long.MaxValue);
            GameSaveSerializer serializer = new();

            DecodedGameSave decoded = serializer.Deserialize(serializer.Serialize(state, SavedAtUtc));

            Assert.That(decoded.SavedAtUtc, Is.EqualTo(SavedAtUtc));
            Assert.That(decoded.State.Revision, Is.EqualTo(long.MaxValue));
            Assert.That(decoded.State.Calendar.TotalHours, Is.EqualTo(CalendarState.MaximumTotalHours));
            Assert.That(decoded.State.Random.MasterSeed, Is.EqualTo(ulong.MaxValue));
            Assert.That(decoded.State.Random.CaptureStreams(), Has.Count.EqualTo(1));
            Assert.That(decoded.State.Random.CaptureStreams()[0].State, Is.EqualTo(ulong.MaxValue));
        }

        [Test]
        public void SerializationSortsMaterializedStreamsByStableName()
        {
            RandomStreamState first = RandomStreamState.Restore(
                StableId.Create("z.stream"),
                DeterministicRandom.AlgorithmVersion,
                1);
            RandomStreamState second = RandomStreamState.Restore(
                StableId.Create("a.stream"),
                DeterministicRandom.AlgorithmVersion,
                2);
            GameState state = new(
                new CalendarState(new GameDate(2026, 1, 1, 0)),
                RandomState.Restore(7, new[] { first, second }));
            GameSaveSerializer serializer = new();

            string json = serializer.Serialize(state, SavedAtUtc);

            Assert.That(json.IndexOf("a.stream", StringComparison.Ordinal),
                Is.LessThan(json.IndexOf("z.stream", StringComparison.Ordinal)));
        }

        [Test]
        public void ChecksumRejectsModifiedPayload()
        {
            GameSaveSerializer serializer = new();
            string json = serializer.Serialize(CreateState(3), SavedAtUtc);
            string corrupted = json.Replace("\"revision\": 3", "\"revision\": 4", StringComparison.Ordinal);

            Assert.That(
                (Action)(() => serializer.Deserialize(corrupted)),
                Throws.TypeOf<InvalidDataException>().With.Message.Contains("checksum"));
        }

        [Test]
        public void SerializationDoesNotEmitAFormatVersion()
        {
            GameSaveSerializer serializer = new();
            JObject envelope = JObject.Parse(serializer.Serialize(CreateState(0), SavedAtUtc));

            Assert.That(envelope["payload"]!["schemaVersion"], Is.Null);
        }

        [Test]
        public void RoundTripPreservesTheCharacterModel()
        {
            CharacterState character = new(
                new CharacterIdentity(StableId.Create("artist.1"), StableId.Create("start.bottom")),
                new CharacterResources(
                    new BoundedResource(450, 1_000),
                    new BoundedResource(300, 800),
                    new BoundedResource(700, 1_200)),
                Money.FromMinorUnits(12_345),
                new AudienceState(new[]
                {
                    new AudienceSegment(StableId.Create("fans.local"), 25),
                    new AudienceSegment(StableId.Create("fans.underground"), 11)
                }),
                new HypeState(new BoundedResource(425, 1_000)),
                new SkillBook(new[]
                {
                    new SkillProgress(StableId.Create("skill.lyrics"), 500, 48)
                }),
                new TalentSet(new[]
                {
                    StableId.Create("talent.wordsmith")
                }),
                new StatusEffectSet(new[]
                {
                    new StatusEffectState(
                        StableId.Create("effect.1"),
                        StableId.Create("status.lovestruck"),
                        StableId.Create("event.romance"),
                        24,
                        72,
                        2)
                }));
            GameState state = new(
                new CalendarState(new GameDate(2026, 1, 1, 8)),
                new RandomState(123),
                character);
            GameSaveSerializer serializer = new();

            DecodedGameSave decoded = serializer.Deserialize(serializer.Serialize(state, SavedAtUtc));

            Assert.That(decoded.State.Character.Identity.StartTemplateId, Is.EqualTo(StableId.Create("start.bottom")));
            Assert.That(decoded.State.Character.Resources.Satiety.Maximum, Is.EqualTo(800));
            Assert.That(decoded.State.Character.Wallet.MinorUnits, Is.EqualTo(12_345));
            Assert.That(decoded.State.Character.Audience.TotalFans, Is.EqualTo(36));
            Assert.That(decoded.State.Character.Skills.Entries[0].Experience, Is.EqualTo(500));
            Assert.That(decoded.State.Character.Talents.Ids[0], Is.EqualTo(StableId.Create("talent.wordsmith")));
            Assert.That(decoded.State.Character.StatusEffects.Effects[0].ExpiresAtTotalHours, Is.EqualTo(72));
        }

        [Test]
        public void RoundTripPreservesAnActiveActivity()
        {
            ActivitySessionState activity = new(
                StableId.Create("activity.1"),
                StableId.Create("work.courier"),
                12,
                4,
                2);
            GameState state = new(
                new CalendarState(new GameDate(2026, 1, 1, 8), 14),
                new RandomState(123),
                CharacterState.CreateDefault(),
                activity);
            GameSaveSerializer serializer = new();

            DecodedGameSave decoded = serializer.Deserialize(serializer.Serialize(state, SavedAtUtc));

            Assert.That(decoded.State.ActiveActivity, Is.Not.Null);
            Assert.That(decoded.State.ActiveActivity!.DefinitionId, Is.EqualTo(StableId.Create("work.courier")));
            Assert.That(decoded.State.ActiveActivity.ElapsedHours, Is.EqualTo(2));
            Assert.That(decoded.State.ActiveActivity.RemainingHours, Is.EqualTo(2));
        }

        [Test]
        public void VersionedPayloadIsRejectedAsAnIncompatibleDevelopmentSave()
        {
            JObject payload = CreateCurrentPayload(0);
            payload["schemaVersion"] = 1;
            GameSaveSerializer serializer = new();

            Assert.That(
                (Action)(() => serializer.Deserialize(serializer.CreateEnvelope(payload))),
                Throws.TypeOf<InvalidDataException>());
        }

        [Test]
        public void UnknownCurrentFieldIsRejected()
        {
            JObject payload = CreateCurrentPayload(0);
            payload["unknownState"] = true;
            GameSaveSerializer serializer = new();

            Assert.That(
                (Action)(() => serializer.Deserialize(serializer.CreateEnvelope(payload))),
                Throws.TypeOf<InvalidDataException>());
        }

        private static JObject CreateCurrentPayload(long revision)
        {
            GameSaveSerializer serializer = new();
            JObject envelope = JObject.Parse(serializer.Serialize(CreateState(revision), SavedAtUtc));
            return (JObject)envelope["payload"]!;
        }

        private static GameState CreateState(long revision)
        {
            return new GameState(
                new CalendarState(new GameDate(2026, 1, 1, 8), 12),
                new RandomState(123),
                revision);
        }
    }
}
