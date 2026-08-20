using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Application.Persistence;
using RapWay.Domain.State;
using UnityEngine;
using VContainer;

namespace RapWay.Composition.Unity.Persistence
{
    public sealed class GameSaveLifecycleAdapter : MonoBehaviour
    {
        private IGameSaveStore _saveStore;
        private IGameStateSnapshotSource _snapshotSource;
        private CancellationTokenSource _lifetimeCancellation;

        [Inject]
        public void Construct(IGameSaveStore saveStore, IGameStateSnapshotSource snapshotSource)
        {
            _saveStore = saveStore ?? throw new ArgumentNullException(nameof(saveStore));
            _snapshotSource = snapshotSource ?? throw new ArgumentNullException(nameof(snapshotSource));
            _lifetimeCancellation = new CancellationTokenSource();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                SaveObservedAsync("pause", _lifetimeCancellation.Token).Forget();
            }
        }

        private void OnApplicationQuit()
        {
            SaveObservedAsync("quit", CancellationToken.None).Forget();
        }

        private void OnDestroy()
        {
            _lifetimeCancellation?.Cancel();
            _lifetimeCancellation?.Dispose();
        }

        private async UniTaskVoid SaveObservedAsync(string reason, CancellationToken cancellationToken)
        {
            if (!_snapshotSource.TryGetStateSnapshot(out GameState stateSnapshot))
            {
                return;
            }

            try
            {
                await _saveStore.SaveAsync(stateSnapshot, DateTime.UtcNow, cancellationToken);
                Debug.Log($"[Persistence] Career saved on {reason}.");
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
