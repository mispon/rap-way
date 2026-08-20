using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using RapWay.Application.Persistence;
using RapWay.Domain.Events;
using RapWay.Domain.State;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Persistence
{
    public sealed class CommittedEventAutosaveScheduler : IStartable, IDisposable
    {
        private static readonly TimeSpan DebounceDelay = TimeSpan.FromSeconds(2);

        private readonly ISubscriber<IDomainEvent> _subscriber;
        private readonly IGameSaveStore _saveStore;
        private readonly IGameStateSnapshotSource _snapshotSource;
        private readonly CancellationTokenSource _lifetimeCancellation = new();
        private CancellationTokenSource _debounceCancellation;
        private IDisposable _subscription;

        public CommittedEventAutosaveScheduler(
            ISubscriber<IDomainEvent> subscriber,
            IGameSaveStore saveStore,
            IGameStateSnapshotSource snapshotSource)
        {
            _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
            _saveStore = saveStore ?? throw new ArgumentNullException(nameof(saveStore));
            _snapshotSource = snapshotSource ?? throw new ArgumentNullException(nameof(snapshotSource));
        }

        public void Start()
        {
            _subscription = _subscriber.Subscribe(_ => Schedule());
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _debounceCancellation?.Cancel();
            _debounceCancellation?.Dispose();
            _lifetimeCancellation.Cancel();
            _lifetimeCancellation.Dispose();
        }

        private void Schedule()
        {
            _debounceCancellation?.Cancel();
            _debounceCancellation?.Dispose();
            _debounceCancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCancellation.Token);
            SaveAfterDebounceAsync(_debounceCancellation.Token).Forget();
        }

        private async UniTaskVoid SaveAfterDebounceAsync(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(DebounceDelay, cancellationToken: cancellationToken);
                if (!_snapshotSource.TryGetStateSnapshot(out GameState stateSnapshot))
                {
                    return;
                }

                await _saveStore.SaveAsync(stateSnapshot, DateTime.UtcNow, cancellationToken);
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
