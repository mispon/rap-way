using System;
using System.Security.Cryptography;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Application.Activities;
using RapWay.Application.Commands;
using RapWay.Application.Events;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using RapWay.Application.Simulation;
using RapWay.Domain.Common;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using UnityEngine;
using VContainer.Unity;

namespace RapWay.Composition.Unity.Persistence
{
    public sealed class GameSessionCoordinator : IAsyncStartable, IGameStateSnapshotSource, IActivityLoop
    {
        private readonly IGameSaveStore _saveStore;
        private readonly ICommittedEventSink _eventSink;
        private readonly IGameSessionLaunchRequest _launchRequest;
        private readonly IActivityDefinitionLookup _activityDefinitions;
        private SimulationSession _session;

        public GameSessionCoordinator(
            IGameSaveStore saveStore,
            ICommittedEventSink eventSink,
            IGameSessionLaunchRequest launchRequest,
            IActivityDefinitionLookup activityDefinitions)
        {
            _saveStore = saveStore ?? throw new ArgumentNullException(nameof(saveStore));
            _eventSink = eventSink ?? throw new ArgumentNullException(nameof(eventSink));
            _launchRequest = launchRequest ?? throw new ArgumentNullException(nameof(launchRequest));
            _activityDefinitions = activityDefinitions ?? throw new ArgumentNullException(nameof(activityDefinitions));
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            GameSessionLaunchRequestData launchRequest = _launchRequest.Consume();
            if (launchRequest.Mode == GameSessionLaunchMode.MainMenu)
            {
                return;
            }

            if (launchRequest.Mode == GameSessionLaunchMode.NewCareer)
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

            GameState replacementState = CreateBootstrapState();
            await _saveStore.SaveAsync(replacementState, DateTime.UtcNow, cancellationToken);
            _session = new SimulationSession(replacementState, _eventSink);
            Debug.LogWarning($"[Persistence] Replaced an incompatible development save: {result.Detail}");
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

        public CommandResult Start(StableId definitionId, int durationHours)
        {
            return GetSession().Execute(
                new StartActivityCommand(definitionId, durationHours),
                new StartActivityCommandHandler(_activityDefinitions));
        }

        public CommandResult AdvanceHour()
        {
            return GetSession().Execute(
                new AdvanceActivityHourCommand(),
                new AdvanceActivityHourCommandHandler(_activityDefinitions));
        }

        public CommandResult Complete()
        {
            return GetSession().Execute(
                new CompleteActivityCommand(),
                new CompleteActivityCommandHandler(_activityDefinitions));
        }

        public CommandResult Interrupt()
        {
            return GetSession().Execute(
                new InterruptActivityCommand(),
                new InterruptActivityCommandHandler(_activityDefinitions));
        }

        private SimulationSession GetSession()
        {
            return _session ?? throw new InvalidOperationException("A game session has not been created.");
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
