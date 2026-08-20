using System;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Application.Events;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using RapWay.Application.Simulation;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Persistence
{
    public sealed class GameSessionCoordinator : IAsyncStartable, IGameStateSnapshotSource
    {
        private readonly IGameSaveStore _saveStore;
        private readonly ICommittedEventSink _eventSink;
        private readonly IGameSessionLaunchRequest _launchRequest;
        private SimulationSession _session;

        public GameSessionCoordinator(
            IGameSaveStore saveStore,
            ICommittedEventSink eventSink,
            IGameSessionLaunchRequest launchRequest)
        {
            _saveStore = saveStore ?? throw new ArgumentNullException(nameof(saveStore));
            _eventSink = eventSink ?? throw new ArgumentNullException(nameof(eventSink));
            _launchRequest = launchRequest ?? throw new ArgumentNullException(nameof(launchRequest));
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            GameSessionLaunchMode launchMode = _launchRequest.Consume();
            if (launchMode == GameSessionLaunchMode.MainMenu)
            {
                return;
            }

            if (launchMode == GameSessionLaunchMode.NewCareer)
            {
                _session = new SimulationSession(CreateBootstrapState(), _eventSink);
                return;
            }

            GameLoadResult result = await _saveStore.LoadAsync(cancellationToken);
            if (result.IsSuccess)
            {
                _session = new SimulationSession(result.State!, _eventSink);
                if (result.Source != GameLoadSource.Primary)
                {
                    Debug.LogWarning($"[Persistence] Recovered career from {result.Source}.");
                }

                return;
            }

            if (result.Failure == GameLoadFailure.NotFound)
            {
                _session = new SimulationSession(CreateBootstrapState(), _eventSink);
                return;
            }

            Debug.LogError($"[Persistence] Career save could not be recovered: {result.Detail}");
        }

        public bool TryGetStateSnapshot(out GameState stateSnapshot)
        {
            if (_session == null)
            {
                stateSnapshot = null;
                return false;
            }

            stateSnapshot = _session.GetStateSnapshot();
            return true;
        }

        private static GameState CreateBootstrapState()
        {
            byte[] seedBytes = new byte[sizeof(ulong)];
            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(seedBytes);
            }

            ulong seed = BitConverter.ToUInt64(seedBytes, 0);
            return GameState.Create(new GameDate(2026, 1, 1, 8), seed);
        }
    }
}
