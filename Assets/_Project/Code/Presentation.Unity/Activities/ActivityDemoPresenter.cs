using System;
using RapWay.Application.Activities;
using RapWay.Application.Commands;
using RapWay.Core.Localization;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.State;
using RapWay.Presentation.Unity.Localization;
using RapWay.Presentation.Unity.Shell;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;

namespace RapWay.Presentation.Unity.Activities
{
    public sealed class ActivityDemoPresenter : MonoBehaviour
    {
        private IActivityLoop _activityLoop;
        private IGameLocalizationService _localizationService;
        private IActivityDefinitionCatalog _definitions;
        private IAppShellFlow _appShellFlow;
        private UiToolkitPresentationSettings _uiToolkitPresentationSettings;
        private ActivityDemoView _view;
        private float _secondsUntilNextHour;
        private bool _isWorkSessionRunning;
        private ActivityDefinition _selectedDefinition;

        private const float SecondsPerGameHour = 2f;

        public void Initialize(
            IActivityLoop activityLoop,
            IActivityDefinitionCatalog definitions,
            IGameLocalizationService localizationService,
            IAppShellFlow appShellFlow,
            UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            _activityLoop = activityLoop ?? throw new ArgumentNullException(nameof(activityLoop));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _appShellFlow = appShellFlow ?? throw new ArgumentNullException(nameof(appShellFlow));
            _uiToolkitPresentationSettings = uiToolkitPresentationSettings ?? throw new ArgumentNullException(nameof(uiToolkitPresentationSettings));
            _appShellFlow.ActivitySelectionRequested += ShowActivitySelection;
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
            if (_appShellFlow != null)
            {
                _appShellFlow.ActivitySelectionRequested -= ShowActivitySelection;
            }

            if (_view == null)
            {
                return;
            }

            _view.StartRequested -= HandleStartRequested;
            _view.ActivitySelected -= HandleActivitySelected;
            _view.Closed -= HandleClosed;
        }

        private void ShowActivitySelection()
        {
            if (_view == null || !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                return;
            }

            _isWorkSessionRunning = false;
            _selectedDefinition = null;
            _view.ShowSelection(state);
        }

        private void HandleActivitySelected(StableId definitionId)
        {
            if (!_definitions.TryGet(definitionId, out ActivityDefinition definition) ||
                !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                return;
            }

            _selectedDefinition = definition;
            _view.ShowConfirmation(definition, state);
        }

        private void HandleStartRequested(StableId definitionId, int durationHours)
        {
            CommandResult result = _activityLoop.Start(definitionId, durationHours);
            if (!result.IsSuccess || !_activityLoop.TryGetStateSnapshot(out GameState state))
            {
                ShowFailure(result);
                return;
            }

            _secondsUntilNextHour = SecondsPerGameHour;
            _isWorkSessionRunning = true;
            _appShellFlow.UpdateHud(state);
            _view.ShowWorkSession(_selectedDefinition, state);
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
                ShowFailure(advanceResult);
                return;
            }

            _appShellFlow.UpdateHud(state);
            if (!state.ActiveActivity.IsComplete)
            {
                _view.ShowWorkSession(_selectedDefinition, state);
                return;
            }

            int completedDurationHours = state.ActiveActivity.DurationHours;
            CommandResult completeResult = _activityLoop.Complete();
            _isWorkSessionRunning = false;
            if (!completeResult.IsSuccess || !_activityLoop.TryGetStateSnapshot(out GameState completedState))
            {
                ShowFailure(completeResult);
                return;
            }

            _appShellFlow.UpdateHud(completedState);
            _view.ShowResults(_selectedDefinition, completedDurationHours, completedState);
        }

        private void ShowFailure(CommandResult result)
        {
            if (_view == null)
            {
                return;
            }

            string failureCode = result.Failure?.Code.Value ?? "unknown";
            _view.ShowFailure(GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoCommandFailed,
                new LocalizationArgument("failure", failureCode)));
        }

        private void HandleClosed()
        {
            _isWorkSessionRunning = false;
            _selectedDefinition = null;
            _view.Hide();
            _appShellFlow.ShowHud();
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
            _view.Closed += HandleClosed;
            _view.Hide();
            _appShellFlow.UpdateHud(state);
        }

        private string GetText(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            return _localizationService.Get(key, arguments);
        }
    }
}
