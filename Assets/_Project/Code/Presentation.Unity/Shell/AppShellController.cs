using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RapWay.Application.Navigation;
using RapWay.Application.Persistence;
using RapWay.Application.Session;
using RapWay.Presentation.Unity.Localization;
using UnityEngine;

namespace RapWay.Presentation.Unity.Shell
{
    public sealed class AppShellController : IAppShellFlow, IDisposable
    {
        private readonly ISceneNavigator _sceneNavigator;
        private readonly ICareerSaveAvailabilityProbe _saveAvailabilityProbe;
        private readonly IGameSessionLaunchRequest _launchRequest;
        private readonly IGameLocalizationService _localizationService;
        private readonly AppShellView _view;

        private AppShellState _state;
        private CancellationTokenSource _transitionCts;

        public AppShellController(
            ISceneNavigator sceneNavigator,
            ICareerSaveAvailabilityProbe saveAvailabilityProbe,
            IGameSessionLaunchRequest launchRequest,
            IGameLocalizationService localizationService)
        {
            _sceneNavigator = sceneNavigator ?? throw new ArgumentNullException(nameof(sceneNavigator));
            _saveAvailabilityProbe = saveAvailabilityProbe ?? throw new ArgumentNullException(nameof(saveAvailabilityProbe));
            _launchRequest = launchRequest ?? throw new ArgumentNullException(nameof(launchRequest));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _transitionCts = new CancellationTokenSource();

            _view = AppShellView.Create();
            _view.Initialize(this, _localizationService);

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
                selectedStartTemplateId: null,
                isBusy: false,
                statusText: _localizationService.Get(GameLocalizationKeys.UiShell.SplashPreparingStage)));
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
                selectedStartTemplateId: null,
                isBusy: false,
                statusText: _localizationService.Get(
                    canContinue ? GameLocalizationKeys.UiShell.MainMenuStatusCanContinue : GameLocalizationKeys.UiShell.MainMenuStatusStartFirstCareer)));
        }

        public void ShowNewCareerTemplateSelection()
        {
            Render(new AppShellState(
                AppShellScreen.NewCareerTemplateSelection,
                AppShellModal.None,
                _state.CanContinue,
                selectedStartTemplateId: CareerStartTemplateId.OnYourOwn,
                isBusy: false,
                statusText: _localizationService.Get(GameLocalizationKeys.UiShell.TemplateStatusChooseCircumstances)));
        }

        public void ShowHud()
        {
            Render(new AppShellState(
                AppShellScreen.Hud,
                AppShellModal.None,
                _state.CanContinue,
                _state.SelectedStartTemplateId,
                isBusy: false,
                statusText: _localizationService.Get(GameLocalizationKeys.UiShell.HudStatusCareerSessionRunning)));
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
                AppShellModal.None when _state.ActiveScreen == AppShellScreen.NewCareerTemplateSelection => AppShellModal.None,
                AppShellModal.None when _state.ActiveScreen == AppShellScreen.Hud => AppShellModal.SessionMenu,
                _ => AppShellModal.None
            };

            if (nextModal == AppShellModal.None && _state.ActiveScreen == AppShellScreen.NewCareerTemplateSelection)
            {
                Render(_state.With(
                    activeScreen: AppShellScreen.MainMenu,
                    clearSelectedStartTemplateId: true,
                    statusText: _localizationService.Get(
                        _state.CanContinue ? GameLocalizationKeys.UiShell.MainMenuStatusCanContinue : GameLocalizationKeys.UiShell.MainMenuStatusStartFirstCareer)));
                return;
            }

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
            ShowNewCareerTemplateSelection();
        }

        public void HandleContinueRequested()
        {
            if (!_state.CanContinue)
            {
                return;
            }

            RunObservedAsync(ContinueCareerAsync);
        }

        public void HandleStartTemplateSelected(CareerStartTemplateId templateId)
        {
            AppShellStartTemplateDefinition definition = AppShellStartTemplateCatalog.Get(templateId);
            Render(_state.With(
                selectedStartTemplateId: templateId,
                statusText: _localizationService.Get(definition.EmphasisKey)));
        }

        public void HandleStartTemplateConfirmed()
        {
            if (!_state.SelectedStartTemplateId.HasValue)
            {
                return;
            }

            CareerStartTemplateId selectedTemplateId = _state.SelectedStartTemplateId.Value;
            RunObservedAsync(cancellationToken => StartNewCareerAsync(selectedTemplateId, cancellationToken));
        }

        private async UniTask ReturnToMainMenuAsync(CancellationToken cancellationToken)
        {
            _launchRequest.Request(GameSessionLaunchMode.MainMenu);
            Render(_state.With(
                activeModal: AppShellModal.None,
                clearSelectedStartTemplateId: true,
                isBusy: true,
                statusText: _localizationService.Get(GameLocalizationKeys.UiShell.StatusReturningToMainMenu)));

            await _sceneNavigator.LoadSceneAsync("Game", cancellationToken);
        }

        private async UniTask StartNewCareerAsync(CareerStartTemplateId templateId, CancellationToken cancellationToken)
        {
            AppShellStartTemplateDefinition definition = AppShellStartTemplateCatalog.Get(templateId);
            _launchRequest.Request(GameSessionLaunchMode.NewCareer, templateId);
            Render(_state.With(
                activeModal: AppShellModal.None,
                selectedStartTemplateId: templateId,
                isBusy: true,
                statusText: _localizationService.Get(
                    GameLocalizationKeys.UiShell.StatusStartingNewCareer,
                    new LocalizationArgument("templateTitle", _localizationService.Get(definition.TitleKey)))));

            await _sceneNavigator.LoadSceneAsync("Game", cancellationToken);
        }

        private async UniTask ContinueCareerAsync(CancellationToken cancellationToken)
        {
            _launchRequest.Request(GameSessionLaunchMode.Continue);
            Render(_state.With(
                activeModal: AppShellModal.None,
                isBusy: true,
                statusText: _localizationService.Get(GameLocalizationKeys.UiShell.StatusLoadingLatestCareer)));

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
                    statusText: _localizationService.Get(GameLocalizationKeys.UiShell.StatusGenericError)));
            }
        }

        private void Render(AppShellState state)
        {
            _state = state;
            _view.Render(state);
        }
    }

    internal static class AppShellStartTemplateCatalog
    {
        public static AppShellStartTemplateDefinition Get(CareerStartTemplateId templateId)
        {
            switch (templateId)
            {
                case CareerStartTemplateId.OnYourOwn:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateOnYourOwnTitle, GameLocalizationKeys.UiShell.TemplateOnYourOwnTagline, GameLocalizationKeys.UiShell.TemplateOnYourOwnSummary, GameLocalizationKeys.UiShell.TemplateOnYourOwnEmphasis);
                case CareerStartTemplateId.AtRockBottom:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateAtRockBottomTitle, GameLocalizationKeys.UiShell.TemplateAtRockBottomTagline, GameLocalizationKeys.UiShell.TemplateAtRockBottomSummary, GameLocalizationKeys.UiShell.TemplateAtRockBottomEmphasis);
                case CareerStartTemplateId.PrivilegedStart:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplatePrivilegedStartTitle, GameLocalizationKeys.UiShell.TemplatePrivilegedStartTagline, GameLocalizationKeys.UiShell.TemplatePrivilegedStartSummary, GameLocalizationKeys.UiShell.TemplatePrivilegedStartEmphasis);
                case CareerStartTemplateId.OneMemeWonder:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateOneMemeWonderTitle, GameLocalizationKeys.UiShell.TemplateOneMemeWonderTagline, GameLocalizationKeys.UiShell.TemplateOneMemeWonderSummary, GameLocalizationKeys.UiShell.TemplateOneMemeWonderEmphasis);
                case CareerStartTemplateId.BasementGenius:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateBasementGeniusTitle, GameLocalizationKeys.UiShell.TemplateBasementGeniusTagline, GameLocalizationKeys.UiShell.TemplateBasementGeniusSummary, GameLocalizationKeys.UiShell.TemplateBasementGeniusEmphasis);
                case CareerStartTemplateId.FormerGroupMember:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateFormerGroupMemberTitle, GameLocalizationKeys.UiShell.TemplateFormerGroupMemberTagline, GameLocalizationKeys.UiShell.TemplateFormerGroupMemberSummary, GameLocalizationKeys.UiShell.TemplateFormerGroupMemberEmphasis);
                case CareerStartTemplateId.Protege:
                    return new AppShellStartTemplateDefinition(templateId, GameLocalizationKeys.UiShell.TemplateProtegeTitle, GameLocalizationKeys.UiShell.TemplateProtegeTagline, GameLocalizationKeys.UiShell.TemplateProtegeSummary, GameLocalizationKeys.UiShell.TemplateProtegeEmphasis);
                default:
                    throw new ArgumentOutOfRangeException(nameof(templateId), templateId, "Unknown start template.");
            }
        }
    }

    internal readonly struct AppShellStartTemplateDefinition
    {
        public AppShellStartTemplateDefinition(
            CareerStartTemplateId id,
            LocalizationKey titleKey,
            LocalizationKey taglineKey,
            LocalizationKey summaryKey,
            LocalizationKey emphasisKey)
        {
            Id = id;
            TitleKey = titleKey;
            TaglineKey = taglineKey;
            SummaryKey = summaryKey;
            EmphasisKey = emphasisKey;
        }

        public CareerStartTemplateId Id { get; }

        public LocalizationKey TitleKey { get; }

        public LocalizationKey TaglineKey { get; }

        public LocalizationKey SummaryKey { get; }

        public LocalizationKey EmphasisKey { get; }
    }
}
