using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RapWay.Application.Persistence;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using RapWay.Infrastructure.Persistence;

namespace RapWay.Architecture.Tests
{
    [TestFixture]
    public sealed class JsonGameSaveStoreTests
    {
        private string _directoryPath = null!;

        [SetUp]
        public void SetUp()
        {
            _directoryPath = Path.Combine(Path.GetTempPath(), "rap-way-save-tests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directoryPath))
            {
                Directory.Delete(_directoryPath, true);
            }
        }

        [Test]
        public async Task MissingSaveReportsNotFound()
        {
            using JsonGameSaveStore store = new(_directoryPath);

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(GameLoadFailure.NotFound));
        }

        [Test]
        public async Task SaveAndLoadPrimaryRoundTrip()
        {
            using JsonGameSaveStore store = new(_directoryPath);
            DateTime timestamp = UtcMinute(1);
            await store.SaveAsync(CreateState(7), timestamp, CancellationToken.None);

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Source, Is.EqualTo(GameLoadSource.Primary));
            Assert.That(result.SavedAtUtc, Is.EqualTo(timestamp));
            Assert.That(result.State!.Revision, Is.EqualTo(7));
        }

        [Test]
        public async Task CorruptPrimaryRecoversNewestValidBackup()
        {
            using JsonGameSaveStore store = new(_directoryPath);
            await store.SaveAsync(CreateState(1), UtcMinute(1), CancellationToken.None);
            await store.SaveAsync(CreateState(2), UtcMinute(2), CancellationToken.None);
            await store.SaveAsync(CreateState(3), UtcMinute(3), CancellationToken.None);
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.PrimaryFileName), "corrupt");

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Source, Is.EqualTo(GameLoadSource.Backup1));
            Assert.That(result.State!.Revision, Is.EqualTo(2));
        }

        [Test]
        public async Task CorruptPrimaryAndFirstBackupRecoverSecondBackup()
        {
            using JsonGameSaveStore store = new(_directoryPath);
            await store.SaveAsync(CreateState(1), UtcMinute(1), CancellationToken.None);
            await store.SaveAsync(CreateState(2), UtcMinute(2), CancellationToken.None);
            await store.SaveAsync(CreateState(3), UtcMinute(3), CancellationToken.None);
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.PrimaryFileName), "corrupt");
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.Backup1FileName), "corrupt");

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Source, Is.EqualTo(GameLoadSource.Backup2));
            Assert.That(result.State!.Revision, Is.EqualTo(1));
        }

        [Test]
        public async Task InterruptedTemporaryFileDoesNotOverrideValidPrimary()
        {
            using JsonGameSaveStore store = new(_directoryPath);
            await store.SaveAsync(CreateState(4), UtcMinute(4), CancellationToken.None);
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.TemporaryFileName), "partial");

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.Source, Is.EqualTo(GameLoadSource.Primary));
            Assert.That(result.State!.Revision, Is.EqualTo(4));
        }

        [Test]
        public async Task AllCorruptCandidatesReportNoValidSave()
        {
            Directory.CreateDirectory(_directoryPath);
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.PrimaryFileName), "bad");
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.Backup1FileName), "bad");
            File.WriteAllText(Path.Combine(_directoryPath, JsonGameSaveStore.Backup2FileName), "bad");
            using JsonGameSaveStore store = new(_directoryPath);

            GameLoadResult result = await store.LoadAsync(CancellationToken.None);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(GameLoadFailure.NoValidSave));
            Assert.That(result.Detail, Does.Contain("Primary"));
        }

        private static DateTime UtcMinute(int minute)
        {
            return new DateTime(2026, 8, 20, 12, minute, 0, DateTimeKind.Utc);
        }

        private static GameState CreateState(long revision)
        {
            return new GameState(
                new CalendarState(new GameDate(2026, 1, 1, 8), revision),
                new RandomState((ulong)(revision + 1)),
                revision);
        }
    }
}
