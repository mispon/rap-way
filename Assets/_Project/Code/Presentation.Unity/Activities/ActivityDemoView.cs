using System;
using System.Globalization;
using RapWay.Application.Activities;
using RapWay.Core.Localization;
using RapWay.Domain.Activities;
using RapWay.Domain.Common;
using RapWay.Domain.Localization;
using RapWay.Domain.State;
using RapWay.Presentation.Unity.Localization;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace RapWay.Presentation.Unity.Activities
{
    public sealed class ActivityDemoView : MonoBehaviour
    {
        private const float ScreenPadding = 16f;

        private IGameLocalizationService _localizationService;
        private IActivityDefinitionCatalog _definitions;
        private VisualElement _root;
        private VisualElement _selectionScreen;
        private VisualElement _confirmationScreen;
        private VisualElement _sessionScreen;
        private VisualElement _resultScreen;
        private Label _selectionResourcesLabel;
        private Label _confirmationResourcesLabel;
        private Label _confirmationTitleLabel;
        private Label _confirmationDescriptionLabel;
        private Label _confirmationDetailsLabel;
        private Label _sessionTitleLabel;
        private Label _sessionResourcesLabel;
        private Label _sessionProgressLabel;
        private VisualElement _sessionProgressFill;
        private Label _resultTitleLabel;
        private Label _resultBodyLabel;
        private Button _confirmationStartButton;
        private ActivityDefinition _selectedDefinition;
        private GameState _latestState;
        private VisualElement _safeAreaFrame;
        private Rect _lastSafeArea;
        private Vector2Int _lastResolution;

        public event Action<StableId> ActivitySelected;

        public event Action<StableId, int> StartRequested;

        public event Action Closed;

        public void Initialize(
            IActivityDefinitionCatalog definitions,
            IGameLocalizationService localizationService,
            UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            if (uiToolkitPresentationSettings == null)
            {
                throw new ArgumentNullException(nameof(uiToolkitPresentationSettings));
            }

            uiToolkitPresentationSettings.EnsureValid();

            UIDocument document = gameObject.AddComponent<UIDocument>();
            document.panelSettings = uiToolkitPresentationSettings.PanelSettings;
            document.sortingOrder = 200;

            _root = document.rootVisualElement;
            _root.style.flexGrow = 1f;
            uiToolkitPresentationSettings.ActivityDemoVisualTree.CloneTree(_root);

            BindElements();
            _safeAreaFrame = _root.Q<VisualElement>(className: "activity-root");
            RefreshSafeArea(force: true);
            AddJobCards();
        }

        private void Update()
        {
            RefreshSafeArea(force: false);
        }

        public void ShowSelection(GameState state)
        {
            _latestState = state ?? throw new ArgumentNullException(nameof(state));
            _selectedDefinition = null;
            RenderResources(_selectionResourcesLabel, state);
            ShowScreen(_selectionScreen);
        }

        public void ShowConfirmation(ActivityDefinition definition, GameState state)
        {
            _selectedDefinition = definition ?? throw new ArgumentNullException(nameof(definition));
            _latestState = state ?? throw new ArgumentNullException(nameof(state));
            RenderResources(_confirmationResourcesLabel, state);
            _confirmationTitleLabel.text = GetText(definition.TitleLocalizationKey);
            _confirmationDescriptionLabel.text = GetText(definition.DescriptionLocalizationKey);
            _confirmationDetailsLabel.text = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoConfirmationDetails,
                new LocalizationArgument("duration", definition.MinimumDurationHours),
                new LocalizationArgument("payment", FormatMoney(definition.CalculatePayment(definition.MinimumDurationHours).MinorUnits)));
            _confirmationStartButton.text = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoStart,
                new LocalizationArgument("duration", definition.MinimumDurationHours));
            ShowScreen(_confirmationScreen);
        }

        public void ShowWorkSession(ActivityDefinition definition, GameState state)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            ActivitySessionState activity = state?.ActiveActivity ??
                                            throw new InvalidOperationException("A work session requires an active activity.");
            _latestState = state;
            _sessionTitleLabel.text = GetText(definition.TitleLocalizationKey);
            RenderResources(_sessionResourcesLabel, state);
            _sessionProgressLabel.text = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoSessionProgress,
                new LocalizationArgument("elapsed", activity.ElapsedHours),
                new LocalizationArgument("duration", activity.DurationHours));
            _sessionProgressFill.style.width = Length.Percent(100f * activity.ElapsedHours / activity.DurationHours);
            ShowScreen(_sessionScreen);
        }

        public void ShowResults(ActivityDefinition definition, int durationHours, GameState state)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            _latestState = state ?? throw new ArgumentNullException(nameof(state));
            _resultTitleLabel.text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoResultTitle);
            _resultBodyLabel.text = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoResultBody,
                new LocalizationArgument("title", GetText(definition.TitleLocalizationKey)),
                new LocalizationArgument("duration", durationHours),
                new LocalizationArgument("payment", FormatMoney(definition.CalculatePayment(durationHours).MinorUnits)));
            ShowScreen(_resultScreen);
        }

        public void ShowFailure(string message)
        {
            _resultTitleLabel.text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoResultTitle);
            _resultBodyLabel.text = message ?? string.Empty;
            ShowScreen(_resultScreen);
        }

        public void Hide()
        {
            if (_root != null)
            {
                _root.style.display = DisplayStyle.None;
            }
        }

        private void BindElements()
        {
            _selectionScreen = _root.Q<VisualElement>("activity-selection-screen");
            _confirmationScreen = _root.Q<VisualElement>("activity-confirmation-screen");
            _sessionScreen = _root.Q<VisualElement>("activity-session-screen");
            _resultScreen = _root.Q<VisualElement>("activity-result-screen");
            _selectionResourcesLabel = _root.Q<Label>("activity-selection-resources");
            _confirmationResourcesLabel = _root.Q<Label>("activity-confirmation-resources");
            _confirmationTitleLabel = _root.Q<Label>("activity-confirmation-title");
            _confirmationDescriptionLabel = _root.Q<Label>("activity-confirmation-description");
            _confirmationDetailsLabel = _root.Q<Label>("activity-confirmation-details");
            _sessionTitleLabel = _root.Q<Label>("activity-session-title");
            _sessionResourcesLabel = _root.Q<Label>("activity-session-resources");
            _sessionProgressLabel = _root.Q<Label>("activity-session-progress");
            _sessionProgressFill = _root.Q<VisualElement>(className: "activity-progress-fill");
            _resultTitleLabel = _root.Q<Label>("activity-result-title");
            _resultBodyLabel = _root.Q<Label>("activity-result-body");
            _confirmationStartButton = _root.Q<Button>("activity-confirmation-start");

            _root.Q<Label>("activity-selection-title").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoTitle);
            _root.Q<Label>("activity-selection-subtitle").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoSelectionSubtitle);
            _root.Q<Label>("activity-selection-jobs-title").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoSectionJobs);
            _root.Q<Label>("activity-confirmation-heading").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoConfirmationTitle);
            _root.Q<Label>("activity-session-heading").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoSessionTitle);
            _root.Q<Button>("activity-selection-close").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoClose);
            _root.Q<Button>("activity-confirmation-cancel").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoCancel);
            _root.Q<Button>("activity-result-close").text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoResultClose);

            _root.Q<Button>("activity-selection-close").clicked += () => Closed?.Invoke();
            _root.Q<Button>("activity-confirmation-cancel").clicked += () => ShowSelection(_latestState);
            _confirmationStartButton.clicked += StartSelectedActivity;
            _root.Q<Button>("activity-result-close").clicked += () => Closed?.Invoke();
        }

        private void AddJobCards()
        {
            VisualElement jobs = _root.Q<VisualElement>("activity-selection-jobs");
            for (int index = 0; index < _definitions.Definitions.Count; index++)
            {
                ActivityDefinition definition = _definitions.Definitions[index];
                VisualElement card = new();
                card.AddToClassList("activity-job-card");

                Label title = new(GetText(definition.TitleLocalizationKey));
                title.AddToClassList("activity-job-card__title");
                card.Add(title);

                Label description = new(GetText(definition.DescriptionLocalizationKey));
                description.AddToClassList("activity-job-card__description");
                card.Add(description);

                Label payment = new(GetText(
                    GameLocalizationKeys.UiActivities.ActivityDemoPayRate,
                    new LocalizationArgument("payment", FormatMoney(definition.PaymentPerCompletedHour.MinorUnits))));
                payment.AddToClassList("activity-job-card__payment");
                card.Add(payment);

                Button chooseButton = new(() => ActivitySelected?.Invoke(definition.Id));
                chooseButton.text = GetText(GameLocalizationKeys.UiActivities.ActivityDemoChoose);
                chooseButton.AddToClassList("activity-primary-button");
                card.Add(chooseButton);
                jobs.Add(card);
            }
        }

        private void StartSelectedActivity()
        {
            if (_selectedDefinition == null)
            {
                return;
            }

            StartRequested?.Invoke(_selectedDefinition.Id, _selectedDefinition.MinimumDurationHours);
        }

        private void RenderResources(Label target, GameState state)
        {
            target.text = GetText(
                GameLocalizationKeys.UiActivities.ActivityDemoResources,
                new LocalizationArgument("energy", state.Character.Resources.Energy.Current),
                new LocalizationArgument("satiety", state.Character.Resources.Satiety.Current),
                new LocalizationArgument("motivation", state.Character.Resources.Motivation.Current),
                new LocalizationArgument("money", FormatMoney(state.Character.Wallet.MinorUnits)));
        }

        private void ShowScreen(VisualElement screen)
        {
            _root.style.display = DisplayStyle.Flex;
            _selectionScreen.style.display = screen == _selectionScreen ? DisplayStyle.Flex : DisplayStyle.None;
            _confirmationScreen.style.display = screen == _confirmationScreen ? DisplayStyle.Flex : DisplayStyle.None;
            _sessionScreen.style.display = screen == _sessionScreen ? DisplayStyle.Flex : DisplayStyle.None;
            _resultScreen.style.display = screen == _resultScreen ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshSafeArea(bool force)
        {
            if (_safeAreaFrame == null)
            {
                return;
            }

            Rect safeArea = Screen.safeArea;
            Vector2Int resolution = new(Screen.width, Screen.height);
            if (!force && safeArea == _lastSafeArea && resolution == _lastResolution)
            {
                return;
            }

            _lastSafeArea = safeArea;
            _lastResolution = resolution;

            _safeAreaFrame.style.paddingLeft = safeArea.xMin + ScreenPadding;
            _safeAreaFrame.style.paddingRight = Mathf.Max(0f, Screen.width - safeArea.xMax) + ScreenPadding;
            _safeAreaFrame.style.paddingBottom = safeArea.yMin + ScreenPadding;
            _safeAreaFrame.style.paddingTop = Mathf.Max(0f, Screen.height - safeArea.yMax) + ScreenPadding;
        }

        private string GetText(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            return arguments == null || arguments.Length == 0
                ? _localizationService.Get(key)
                : _localizationService.Get(key, arguments);
        }

        private static string FormatMoney(long minorUnits)
        {
            decimal majorUnits = minorUnits / 100m;
            return majorUnits.ToString("N0", CultureInfo.InvariantCulture) + " ₽";
        }
    }
}
