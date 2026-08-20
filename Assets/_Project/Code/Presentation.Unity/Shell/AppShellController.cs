using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Application.Navigation;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using UnityEngine;

namespace RapWay.Presentation.Unity.Shell
{
    public sealed class AppShellController : IAppShellFlow, IDisposable
    {
        private readonly ISceneNavigator _sceneNavigator;
        private readonly ICareerSaveAvailabilityProbe _saveAvailabilityProbe;
        private readonly IGameSessionLaunchRequest _launchRequest;
        private readonly AppShellView _view;

        private AppShellState _state;
        private CancellationTokenSource _transitionCts;

        public AppShellController(
            ISceneNavigator sceneNavigator,
            ICareerSaveAvailabilityProbe saveAvailabilityProbe,
            IGameSessionLaunchRequest launchRequest)
        {
            _sceneNavigator = sceneNavigator ?? throw new ArgumentNullException(nameof(sceneNavigator));
            _saveAvailabilityProbe = saveAvailabilityProbe ?? throw new ArgumentNullException(nameof(saveAvailabilityProbe));
            _launchRequest = launchRequest ?? throw new ArgumentNullException(nameof(launchRequest));
            _transitionCts = new CancellationTokenSource();

            _view = AppShellView.Create();
            _view.Initialize(this);

            ShowSplash();
        }

        public void Dispose()
        {
            _transitionCts.Cancel();
            _transitionCts.Dispose();
        }

        public void ShowSplash()
        {
            Render(new AppShellState(
                AppShellScreen.Splash,
                AppShellModal.None,
                canContinue: false,
                isBusy: false,
                statusText: "Preparing the stage..."));
        }

        public async UniTask ShowMainMenuAsync(CancellationToken cancellationToken)
        {
            bool canContinue = false;

            try
            {
                canContinue = await _saveAvailabilityProbe.HasCareerSaveAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            Render(new AppShellState(
                AppShellScreen.MainMenu,
                AppShellModal.None,
                canContinue,
                isBusy: false,
                statusText: canContinue ? "Pick your next move." : "Start your first career."));
        }

        public void ShowHud()
        {
            Render(new AppShellState(
                AppShellScreen.Hud,
                AppShellModal.None,
                _state.CanContinue,
                isBusy: false,
                statusText: "Career session running."));
        }

        public void HandleBackAction()
        {
            if (_state.IsBusy)
            {
                return;
            }

            AppShellModal nextModal = _state.ActiveModal switch
            {
                AppShellModal.None when _state.ActiveScreen == AppShellScreen.MainMenu => AppShellModal.ExitConfirmation,
                AppShellModal.None when _state.ActiveScreen == AppShellScreen.Hud => AppShellModal.SessionMenu,
                _ => AppShellModal.None
            };

            Render(_state.With(activeModal: nextModal));
        }

        public void HandleModalDismiss()
        {
            if (_state.ActiveModal == AppShellModal.None)
            {
                return;
            }

            Render(_state.With(activeModal: AppShellModal.None));
        }

        public void HandleQuitConfirmed()
        {
            UnityEngine.Application.Quit();
        }

        public void HandleReturnToMenuRequested()
        {
            RunObservedAsync(ReturnToMainMenuAsync);
        }

        public void HandleNewCareerRequested()
        {
            RunObservedAsync(StartNewCareerAsync);
        }

        public void HandleContinueRequested()
        {
            if (!_state.CanContinue)
            {
                return;
            }

            RunObservedAsync(ContinueCareerAsync);
        }

        private async UniTask ReturnToMainMenuAsync(CancellationToken cancellationToken)
        {
            _launchRequest.Request(GameSessionLaunchMode.MainMenu);
            Render(_state.With(
                activeModal: AppShellModal.None,
                isBusy: true,
                statusText: "Returning to the main menu..."));

            await _sceneNavigator.LoadSceneAsync("Game", cancellationToken);
        }

        private async UniTask StartNewCareerAsync(CancellationToken cancellationToken)
        {
            _launchRequest.Request(GameSessionLaunchMode.NewCareer);
            Render(_state.With(
                activeModal: AppShellModal.None,
                isBusy: true,
                statusText: "Starting a new career..."));

            await _sceneNavigator.LoadSceneAsync("Game", cancellationToken);
        }

        private async UniTask ContinueCareerAsync(CancellationToken cancellationToken)
        {
            _launchRequest.Request(GameSessionLaunchMode.Continue);
            Render(_state.With(
                activeModal: AppShellModal.None,
                isBusy: true,
                statusText: "Loading your latest career..."));

            await _sceneNavigator.LoadSceneAsync("Game", cancellationToken);
        }

        private void RunObservedAsync(Func<CancellationToken, UniTask> operation)
        {
            _transitionCts.Cancel();
            _transitionCts.Dispose();
            _transitionCts = new CancellationTokenSource();

            ExecuteAsync(operation, _transitionCts.Token).Forget();
        }

        private async UniTaskVoid ExecuteAsync(Func<CancellationToken, UniTask> operation, CancellationToken cancellationToken)
        {
            try
            {
                await operation(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Render(_state.With(
                    isBusy: false,
                    statusText: "Something went wrong. Check the console."));
            }
        }

        private void Render(AppShellState state)
        {
            _state = state;
            _view.Render(state);
        }
    }
}
