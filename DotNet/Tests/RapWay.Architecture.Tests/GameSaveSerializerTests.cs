using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RapWay.Domain.Common;
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
        public void SchemaZeroFixtureMigratesSequentiallyToCurrent()
        {
            GameSaveSerializer serializer = new();
            string fixturePath = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "Fixtures",
                "game-save-v0.json");

            DecodedGameSave decoded = serializer.Deserialize(File.ReadAllText(fixturePath));

            Assert.That(decoded.State.Revision, Is.Zero);
            Assert.That(decoded.State.Calendar.TotalHours, Is.EqualTo(12));
        }

        [Test]
        public void FutureSchemaIsRejectedWithoutGuessing()
        {
            JObject payload = CreateCurrentPayload(0);
            payload["schemaVersion"] = GameStateSnapshotMapper.CurrentSchemaVersion + 1;
            GameSaveSerializer serializer = new();

            Assert.That(
                (Action)(() => serializer.Deserialize(serializer.CreateEnvelope(payload))),
                Throws.TypeOf<UnsupportedSaveSchemaException>());
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
