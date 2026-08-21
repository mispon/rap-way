using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RapWay.Application.Persistence;
using RapWay.Domain.State;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class JsonGameSaveStore : IGameSaveStore, IDisposable
    {
        public const string PrimaryFileName = "career.save.json";
        public const string Backup1FileName = "career.save.backup1.json";
        public const string Backup2FileName = "career.save.backup2.json";
        public const string TemporaryFileName = "career.save.tmp";

        private readonly string _directoryPath;
        private readonly GameSaveSerializer _serializer;
        private readonly SemaphoreSlim _gate = new(1, 1);

        public JsonGameSaveStore(string directoryPath)
            : this(directoryPath, new GameSaveSerializer())
        {
        }

        public JsonGameSaveStore(string directoryPath, GameSaveSerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException("A save directory is required.", nameof(directoryPath));
            }

            _directoryPath = Path.GetFullPath(directoryPath);
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task SaveAsync(GameState stateSnapshot, DateTime savedAtUtc, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string json = _serializer.Serialize(stateSnapshot, savedAtUtc);
            return SaveCapturedAsync(json, cancellationToken);
        }

        public async Task<GameLoadResult> LoadAsync(CancellationToken cancellationToken)
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await Task.Run(() => LoadCore(cancellationToken), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _gate.Release();
            }
        }

        public void Dispose()
        {
            _gate.Dispose();
        }

        private async Task SaveCapturedAsync(string json, CancellationToken cancellationToken)
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await Task.Run(() => SaveCore(json, cancellationToken), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _gate.Release();
            }
        }

        private void SaveCore(string json, CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_directoryPath);
            string temporaryPath = GetPath(TemporaryFileName);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                using (FileStream stream = new(
                           temporaryPath,
                           FileMode.Create,
                           FileAccess.Write,
                           FileShare.None,
                           4096,
                           FileOptions.WriteThrough))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                }

                cancellationToken.ThrowIfCancellationRequested();
                string primaryPath = GetPath(PrimaryFileName);
                string backup1Path = GetPath(Backup1FileName);
                string backup2Path = GetPath(Backup2FileName);

                MoveReplacing(backup1Path, backup2Path);
                if (File.Exists(primaryPath))
                {
                    File.Replace(temporaryPath, primaryPath, backup1Path);
                }
                else
                {
                    File.Move(temporaryPath, primaryPath);
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
        }

        private GameLoadResult LoadCore(CancellationToken cancellationToken)
        {
            Candidate[] candidates =
            {
                new(GameLoadSource.Primary, GetPath(PrimaryFileName)),
                new(GameLoadSource.Backup1, GetPath(Backup1FileName)),
                new(GameLoadSource.Backup2, GetPath(Backup2FileName))
            };
            List<string> failures = new();
            List<LoadedCandidate> validBackups = new();
            bool anyFileExists = false;

            for (int index = 0; index < candidates.Length; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Candidate candidate = candidates[index];
                if (!File.Exists(candidate.Path))
                {
                    continue;
                }

                anyFileExists = true;
                try
                {
                    DecodedGameSave decoded = _serializer.Deserialize(File.ReadAllText(candidate.Path));
                    if (candidate.Source == GameLoadSource.Primary)
                    {
                        return GameLoadResult.Loaded(decoded.State, decoded.SavedAtUtc, candidate.Source);
                    }

                    validBackups.Add(new LoadedCandidate(candidate.Source, decoded));
                }
                catch (Exception exception) when (exception is IOException ||
                                                  exception is UnauthorizedAccessException ||
                                                  exception is FormatException ||
                                                  exception is InvalidDataException ||
                                                  exception is JsonException ||
                                                  exception is ArgumentException ||
                                                  exception is InvalidOperationException ||
                                                  exception is OverflowException)
                {
                    failures.Add($"{candidate.Source}: {exception.Message}");
                }
            }

            if (validBackups.Count > 0)
            {
                validBackups.Sort((left, right) =>
                {
                    int timestampOrder = right.Save.SavedAtUtc.CompareTo(left.Save.SavedAtUtc);
                    return timestampOrder != 0 ? timestampOrder : left.Source.CompareTo(right.Source);
                });
                LoadedCandidate selected = validBackups[0];
                return GameLoadResult.Loaded(selected.Save.State, selected.Save.SavedAtUtc, selected.Source);
            }

            return anyFileExists
                ? GameLoadResult.Failed(GameLoadFailure.NoValidSave, string.Join(" | ", failures))
                : GameLoadResult.Failed(GameLoadFailure.NotFound, "No save files were found.");
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(_directoryPath, fileName);
        }

        private static void MoveReplacing(string sourcePath, string destinationPath)
        {
            if (!File.Exists(sourcePath))
            {
                return;
            }

            if (File.Exists(destinationPath))
            {
                File.Replace(sourcePath, destinationPath, null);
            }
            else
            {
                File.Move(sourcePath, destinationPath);
            }
        }

        private readonly struct Candidate
        {
            public Candidate(GameLoadSource source, string path)
            {
                Source = source;
                Path = path;
            }

            public GameLoadSource Source { get; }

            public string Path { get; }
        }

        private readonly struct LoadedCandidate
        {
            public LoadedCandidate(GameLoadSource source, DecodedGameSave save)
            {
                Source = source;
                Save = save;
            }

            public GameLoadSource Source { get; }

            public DecodedGameSave Save { get; }
        }
    }
}
