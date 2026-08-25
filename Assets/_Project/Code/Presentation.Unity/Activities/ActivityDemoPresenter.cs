using System;
using RapWay.Application.Activities;
using RapWay.Application.Commands;
using RapWay.Core.Localization;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.State;
using RapWay.Presentation.Unity.Localization;
using RapWay.Presentation.Unity.Navigation;
using RapWay.Presentation.Unity.Shell;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;

namespace RapWay.Presentation.Unity.Activities
{
    public sealed class ActivityDemoPresenter : MonoBehaviour
    {
        private const float SecondsPerGameHour = 2f;

        private IActivityLoop _activityLoop;
        private IGameLocalizationService _localizationService;
        private IActivityDefinitionCatalog _definitions;
        private IAppShellFlow _appShellFlow;
        private IUiNavigator _uiNavigator;
        private UiToolkitPresentationSettings _uiToolkitPresentationSettings;
        private ActivityDemoView _view;
        private float _secondsUntilNextHour;
        private bool _isWorkSessionRunning;
        private StableId _activeDefinitionId;
        private string _failureMessage = string.Empty;

        public void Initialize(
            IActivityLoop activityLoop,
            IActivityDefinitionCatalog definitions,
            IGameLocalizationService localizationService,
            IAppShellFlow appShellFlow,
            IUiNavigator uiNavigator,
            UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            _activityLoop = activityLoop ?? throw new ArgumentNullException(nameof(activityLoop));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _appShellFlow = appShellFlow ?? throw new ArgumentNullException(nameof(appShellFlow));
            _uiNavigator = uiNavigator ?? throw new ArgumentNullException(nameof(uiNavigator));
            _uiToolkitPresentationSettings = uiToolkitPresentationSettings ?? throw new ArgumentNullException(nameof(uiToolkitPresentationSettings));
            _uiNavigator.Changed += HandleNavigationChanged;
            TryCreateView();
        }

        private void Update()
        {
            if (_view == null)
            {
                TryCreateView();
                return;
            }

            AdvanceWorkSession();
        }

        private void OnDestroy()
        {
            if (_uiNavigator != null)
            {
                _uiNavigator.Changed -= HandleNavigationChanged;
            }

            if (_view == null)
            {
                return;
            }

            _view.StartRequested -= HandleStartRequested;
            _view.ActivitySelected -= HandleActivitySelected;
            _view.BackRequested -= HandleBackRequested;
            _view.Closed -= HandleClosed;
        }

        private void HandleActivitySelected(StableId definitionId)
        {
            if (!_definitions.TryGet(definitionId, out ActivityDefinition _))
            {
                return;
            }

            _failureMessage = string.Empty;
            _uiNavigator.Navigate(UiRouteId.ActivityConfirmation, new ActivityUiRouteContext(definitionId.Value));
        }

        private void HandleStartRequested(StableId definitionId, int durationHours)
        {
            CommandResult result = _activityLoop.Start(definitionId, durationHours);
            if (!result.IsSuccess || !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                ShowFailure(result, definitionId, durationHours);
                return;
            }

            _activeDefinitionId = definitionId;
            _secondsUntilNextHour = SecondsPerGameHour;
            _isWorkSessionRunning = true;
            _appShellFlow.UpdateHud(state);
            _uiNavigator.Navigate(UiRouteId.ActivitySession, new ActivityUiRouteContext(definitionId.Value));
        }

        private void AdvanceWorkSession()
        {
            if (!_isWorkSessionRunning)
            {
                return;
            }

            _secondsUntilNextHour -= Time.unscaledDeltaTime;
            if (_secondsUntilNextHour > 0f)
            {
                return;
            }

            _secondsUntilNextHour += SecondsPerGameHour;
            CommandResult advanceResult = _activityLoop.AdvanceHour();
            if (!advanceResult.IsSuccess || !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                _isWorkSessionRunning = false;
                ShowFailure(advanceResult, _activeDefinitionId, durationHours: 0);
                return;
            }

            _appShellFlow.UpdateHud(state);
            if (!state.ActiveActivity.IsComplete)
            {
                RenderActiveSessionIfVisible(state);
                return;
            }

            int completedDurationHours = state.ActiveActivity.DurationHours;
            CommandResult completeResult = _activityLoop.Complete();
            _isWorkSessionRunning = false;
            if (!completeResult.IsSuccess || !_activityLoop.TryGetStateSnapshot(out GameState completedState))
            {
                ShowFailure(completeResult, _activeDefinitionId, completedDurationHours);
                return;
            }

            _appShellFlow.UpdateHud(completedState);
            _uiNavigator.Navigate(
                UiRouteId.ActivityResult,
                new ActivityUiRouteContext(_activeDefinitionId.Value, durationHours: completedDurationHours));
        }

        private void HandleBackRequested()
        {
            _uiNavigator.TryGoBack();
        }

        private void HandleClosed()
        {
            _failureMessage = string.Empty;
            _uiNavigator.GoHome();
        }

        private void HandleNavigationChanged(UiNavigationSnapshot snapshot)
        {
            if (_view == null || snapshot.ActiveDialog != null || snapshot.CurrentRoute == null)
            {
                _view?.Hide();
                return;
            }

            if (!_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                _view.Hide();
                return;
            }

            UiRouteEntry route = snapshot.CurrentRoute;
            switch (route.Definition.Id)
            {
                case UiRouteId.ActivitySelection:
                    _view.ShowSelection(state);
                    break;
                case UiRouteId.ActivityConfirmation:
                    if (TryGetDefinition(route.Context, out ActivityDefinition confirmationDefinition))
                    {
                        _view.ShowConfirmation(confirmationDefinition, state);
                    }
                    else
                    {
                        _view.Hide();
                    }

                    break;
                case UiRouteId.ActivitySession:
                    if (TryGetDefinition(route.Context, out ActivityDefinition sessionDefinition) && state.ActiveActivity != null)
                    {
                        _view.ShowWorkSession(sessionDefinition, state);
                    }
                    else
                    {
                        _view.Hide();
                    }

                    break;
                case UiRouteId.ActivityResult:
                    if (TryGetDefinition(route.Context, out ActivityDefinition resultDefinition) &&
                        route.Context is ActivityUiRouteContext resultContext)
                    {
                        if (string.IsNullOrEmpty(_failureMessage))
                        {
                            _view.ShowResults(resultDefinition, resultContext.DurationHours, state);
                        }
                        else
                        {
                            _view.ShowFailure(_failureMessage);
                        }
                    }
                    else
                    {
                        _view.Hide();
                    }

                    break;
                default:
                    _view.Hide();
                    break;
            }
        }

        private void RenderActiveSessionIfVisible(GameState state)
        {
            UiRouteEntry currentRoute = _uiNavigator.Snapshot.CurrentRoute;
            if (_view == null ||
                currentRoute == null ||
                currentRoute.Definition.Id != UiRouteId.ActivitySession ||
                !TryGetDefinition(currentRoute.Context, out ActivityDefinition definition))
            {
                return;
            }

            _view.ShowWorkSession(definition, state);
        }

        private void ShowFailure(CommandResult result, StableId definitionId, int durationHours)
        {
            string failureCode = result.Failure?.Code.Value ?? "unknown";
            _failureMessage = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoCommandFailed,
                new LocalizationArgument("failure", failureCode));
            _uiNavigator.Navigate(
                UiRouteId.ActivityResult,
                new ActivityUiRouteContext(definitionId.Value, durationHours: durationHours));
        }

        private bool TryGetDefinition(IUiRouteContext routeContext, out ActivityDefinition definition)
        {
            definition = null;
            return routeContext is ActivityUiRouteContext activityContext &&
                   StableId.TryCreate(activityContext.ActivityId, out StableId definitionId) &&
                   _definitions.TryGet(definitionId, out definition);
        }

        private void TryCreateView()
        {
            if (_view != null || !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                return;
            }

            _view = gameObject.AddComponent<ActivityDemoView>();
            _view.Initialize(_definitions, _localizationService, _uiToolkitPresentationSettings);
            _view.StartRequested += HandleStartRequested;
            _view.ActivitySelected += HandleActivitySelected;
            _view.BackRequested += HandleBackRequested;
            _view.Closed += HandleClosed;
            _view.Hide();
            _appShellFlow.UpdateHud(state);
            HandleNavigationChanged(_uiNavigator.Snapshot);
        }

        private string GetText(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            return _localizationService.Get(key, arguments);
        }
    }
}
