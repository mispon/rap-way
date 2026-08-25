using System;
using System.Collections.Generic;
using System.Globalization;
using RapWay.Application.Session;
using RapWay.Core.Localization;
using RapWay.Domain.Localization;
using RapWay.Domain.State;
using RapWay.Presentation.Unity.Localization;
using RapWay.Presentation.Unity.Navigation;
using RapWay.Presentation.Unity.UiToolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace RapWay.Presentation.Unity.Shell
{
    public sealed class AppShellView : MonoBehaviour
    {
        private const string TemplateOptionSelectedClassName = "template-option-button--selected";
        private UIDocument _document;
        private AppShellController _controller;
        private IGameLocalizationService _localizationService;
        private VisualElement _safeAreaFrame;
        private VisualElement _dialogScreen;
        private VisualElement _splashScreen;
        private VisualElement _mainMenuScreen;
        private VisualElement _newCareerTemplateScreen;
        private VisualElement _hudScreen;
        private Label _mainMenuStatusLabel;
        private Label _newCareerStatusLabel;
        private Button _continueButton;
        private Button _menuButton;
        private Button _startTemplateBackButton;
        private Button _startTemplateConfirmButton;
        private Label _dialogTitle;
        private Label _dialogBody;
        private VisualElement _dialogActions;
        private Label _heroStatusLabel;
        private Label _subtitleLabel;
        private Label _templateScreenTitleLabel;
        private Label _templateScreenSubtitleLabel;
        private Label _startTemplateTitleLabel;
        private Label _startTemplateTaglineLabel;
        private Label _startTemplateSummaryLabel;
        private Label _startTemplateEmphasisLabel;
        private Label _hudTitleLabel;
        private Label _hudBodyLabel;
        private Label _hudOpportunityBodyLabel;
        private Label _hudDateLabel;
        private Label _hudEnergyLabel;
        private Label _hudSatietyLabel;
        private Label _hudMotivationLabel;
        private Label _hudMoneyLabel;
        private Label _hudFansLabel;
        private Label _hudHypeLabel;
        private Button _newCareerButton;
        private Button _quitButton;
        private Button _hudActionButton;
        private Dictionary<CareerStartTemplateId, Button> _templateButtons;
        private Rect _lastSafeArea;
        private Vector2Int _lastResolution;

        public static AppShellView Create()
        {
            GameObject shellObject = new("Stage4AppShell");
            DontDestroyOnLoad(shellObject);
            return shellObject.AddComponent<AppShellView>();
        }

        public void Initialize(
            AppShellController controller,
            IGameLocalizationService localizationService,
            UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            if (uiToolkitPresentationSettings == null)
            {
                throw new ArgumentNullException(nameof(uiToolkitPresentationSettings));
            }

            uiToolkitPresentationSettings.EnsureValid();
            EnsureEventSystem();
            EnsureDocument(uiToolkitPresentationSettings);
            BindView();
        }

        public void Render(AppShellState state)
        {
            if (_document == null)
            {
                return;
            }

            _splashScreen.style.display = ToDisplayStyle(state.ActiveScreen == AppShellScreen.Splash);
            _mainMenuScreen.style.display = ToDisplayStyle(state.ActiveScreen == AppShellScreen.MainMenu);
            _newCareerTemplateScreen.style.display = ToDisplayStyle(state.ActiveScreen == AppShellScreen.NewCareerTemplateSelection);
            _hudScreen.style.display = ToDisplayStyle(state.ActiveScreen == AppShellScreen.Hud);

            _continueButton.SetEnabled(state.CanContinue && !state.IsBusy);
            _menuButton.SetEnabled(!state.IsBusy);
            _startTemplateBackButton.SetEnabled(!state.IsBusy);
            _startTemplateConfirmButton.SetEnabled(state.SelectedStartTemplateId.HasValue && !state.IsBusy);
            ApplyStaticLocalizedText();
            string statusText = state.StatusText ?? string.Empty;
            _mainMenuStatusLabel.text = statusText;
            _newCareerStatusLabel.text = statusText;
            ConfigureTemplateSelection(state);

            _dialogScreen.style.display = ToDisplayStyle(state.ActiveDialog != null);
            ConfigureDialog(state);
        }

        public void RenderHud(GameState state)
        {
            if (state == null || _hudDateLabel == null)
            {
                return;
            }

            _hudDateLabel.text = FormatDate(state);
            _hudEnergyLabel.text = FormatPercent(state.Character.Resources.Energy.Current, state.Character.Resources.Energy.Maximum);
            _hudSatietyLabel.text = FormatPercent(state.Character.Resources.Satiety.Current, state.Character.Resources.Satiety.Maximum);
            _hudMotivationLabel.text = FormatPercent(state.Character.Resources.Motivation.Current, state.Character.Resources.Motivation.Maximum);
            _hudMoneyLabel.text = FormatMoney(state.Character.Wallet.MinorUnits);
            _hudFansLabel.text = state.Character.Audience.TotalFans.ToString("N0", CultureInfo.InvariantCulture);
            _hudHypeLabel.text = state.Character.Hype.Intensity.Current.ToString(CultureInfo.InvariantCulture);
        }

        private void Update()
        {
            if (_controller == null)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                _controller.HandleBackAction();
            }

            RefreshSafeAreaIfNeeded();
        }

        private void EnsureDocument(UiToolkitPresentationSettings uiToolkitPresentationSettings)
        {
            if (_document != null)
            {
                return;
            }

            _document = gameObject.AddComponent<UIDocument>();
            _document.panelSettings = uiToolkitPresentationSettings.PanelSettings;
            _document.sortingOrder = 100;

            VisualElement root = _document.rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;
            root.style.width = Length.Percent(100);
            root.style.height = Length.Percent(100);
            uiToolkitPresentationSettings.AppShellVisualTree.CloneTree(root);
        }

        private static void EnsureEventSystem()
        {
            EventSystem existingEventSystem = FindAnyObjectByType<EventSystem>();
            if (existingEventSystem != null)
            {
                return;
            }

            GameObject eventSystemObject = new("App EventSystem");
            DontDestroyOnLoad(eventSystemObject);
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private void BindView()
        {
            VisualElement root = _document.rootVisualElement;

            _safeAreaFrame = root.Q<VisualElement>("safe-area-frame");
            _dialogScreen = root.Q<VisualElement>("screen-dialog");
            _splashScreen = root.Q<VisualElement>("screen-splash");
            _mainMenuScreen = root.Q<VisualElement>("screen-main-menu");
            _newCareerTemplateScreen = root.Q<VisualElement>("screen-new-career-template");
            _hudScreen = root.Q<VisualElement>("screen-hud");
            _mainMenuStatusLabel = root.Q<Label>("main-menu-status-label");
            _newCareerStatusLabel = root.Q<Label>("new-career-status-label");
            _continueButton = root.Q<Button>("continue-button");
            _menuButton = root.Q<Button>("hud-menu-button");
            _startTemplateBackButton = root.Q<Button>("template-back-button");
            _startTemplateConfirmButton = root.Q<Button>("template-confirm-button");
            _dialogTitle = root.Q<Label>("dialog-title");
            _dialogBody = root.Q<Label>("dialog-body");
            _dialogActions = root.Q<VisualElement>("dialog-actions");
            _heroStatusLabel = root.Q<Label>(className: "status-label--hero");
            _subtitleLabel = root.Q<Label>(className: "subtitle-label");
            _templateScreenTitleLabel = root.Q<Label>(className: "template-screen-title");
            _templateScreenSubtitleLabel = root.Q<Label>(className: "template-screen-subtitle");
            _startTemplateTitleLabel = root.Q<Label>("template-detail-title");
            _startTemplateTaglineLabel = root.Q<Label>("template-detail-tagline");
            _startTemplateSummaryLabel = root.Q<Label>("template-detail-summary");
            _startTemplateEmphasisLabel = root.Q<Label>("template-detail-emphasis");
            _hudTitleLabel = root.Q<Label>(className: "hud-title");
            _hudBodyLabel = root.Q<Label>("hud-location-body");
            _hudOpportunityBodyLabel = root.Q<Label>("hud-opportunity-body");
            _hudDateLabel = root.Q<Label>("hud-date-label");
            _hudEnergyLabel = root.Q<Label>("hud-energy-value");
            _hudSatietyLabel = root.Q<Label>("hud-satiety-value");
            _hudMotivationLabel = root.Q<Label>("hud-motivation-value");
            _hudMoneyLabel = root.Q<Label>("hud-money-value");
            _hudFansLabel = root.Q<Label>("hud-fans-value");
            _hudHypeLabel = root.Q<Label>("hud-hype-value");

            _newCareerButton = root.Q<Button>("new-career-button");
            _quitButton = root.Q<Button>("quit-button");
            _hudActionButton = root.Q<Button>("hud-action-button");
            _templateButtons = new Dictionary<CareerStartTemplateId, Button>
            {
                { CareerStartTemplateId.OnYourOwn, root.Q<Button>("template-on-your-own-button") },
                { CareerStartTemplateId.AtRockBottom, root.Q<Button>("template-at-rock-bottom-button") },
                { CareerStartTemplateId.PrivilegedStart, root.Q<Button>("template-privileged-start-button") },
                { CareerStartTemplateId.OneMemeWonder, root.Q<Button>("template-one-meme-wonder-button") },
                { CareerStartTemplateId.BasementGenius, root.Q<Button>("template-basement-genius-button") },
                { CareerStartTemplateId.FormerGroupMember, root.Q<Button>("template-former-group-member-button") },
                { CareerStartTemplateId.Protege, root.Q<Button>("template-protege-button") }
            };
            _newCareerButton.clicked += _controller.HandleNewCareerRequested;
            _continueButton.clicked += _controller.HandleContinueRequested;
            _quitButton.clicked += _controller.HandleBackAction;
            _menuButton.clicked += _controller.HandleBackAction;
            _startTemplateBackButton.clicked += _controller.HandleBackAction;
            _startTemplateConfirmButton.clicked += _controller.HandleStartTemplateConfirmed;
            _hudActionButton.clicked += _controller.RequestActivitySelection;

            RegisterTemplateCallbacks();
            RefreshSafeArea(force: true);
        }

        private void ConfigureDialog(AppShellState state)
        {
            DialogUiRouteContext dialogContext = state.ActiveDialog;
            if (dialogContext == null)
            {
                return;
            }

            _dialogTitle.text = GetText(dialogContext.TitleKey);
            _dialogBody.text = GetText(dialogContext.BodyKey);
            _dialogActions.Clear();

            foreach (UiDialogAction action in dialogContext.Actions)
            {
                string actionId = action.Id;
                Button actionButton = new()
                {
                    text = GetText(action.LabelKey)
                };
                actionButton.AddToClassList(action.IsPrimary ? "primary-button" : "ghost-button");
                actionButton.clicked += () => _controller.HandleDialogAction(actionId);
                _dialogActions.Add(actionButton);
            }
        }

        private void RefreshSafeAreaIfNeeded()
        {
            Rect safeArea = Screen.safeArea;
            Vector2Int resolution = new(Screen.width, Screen.height);
            if (safeArea == _lastSafeArea && resolution == _lastResolution)
            {
                return;
            }

            RefreshSafeArea(force: true);
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

            float left = safeArea.xMin;
            float right = Mathf.Max(0f, Screen.width - safeArea.xMax);
            float bottom = safeArea.yMin;
            float top = Mathf.Max(0f, Screen.height - safeArea.yMax);

            _safeAreaFrame.style.paddingLeft = left;
            _safeAreaFrame.style.paddingRight = right;
            _safeAreaFrame.style.paddingBottom = bottom;
            _safeAreaFrame.style.paddingTop = top;
        }

        private static DisplayStyle ToDisplayStyle(bool isVisible)
        {
            return isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void ConfigureTemplateSelection(AppShellState state)
        {
            if (_templateButtons == null || _templateButtons.Count == 0)
            {
                return;
            }

            CareerStartTemplateId selectedTemplateId = state.SelectedStartTemplateId ?? CareerStartTemplateId.OnYourOwn;
            AppShellStartTemplateDefinition definition = AppShellStartTemplateCatalog.Get(selectedTemplateId);

            _startTemplateTitleLabel.text = GetText(definition.TitleKey);
            _startTemplateTaglineLabel.text = GetText(definition.TaglineKey);
            _startTemplateSummaryLabel.text = GetText(definition.SummaryKey);
            _startTemplateEmphasisLabel.text = GetText(definition.EmphasisKey);

            foreach (KeyValuePair<CareerStartTemplateId, Button> pair in _templateButtons)
            {
                bool isSelected = pair.Key == selectedTemplateId;
                pair.Value.EnableInClassList(TemplateOptionSelectedClassName, isSelected);
            }
        }

        private void RegisterTemplateCallbacks()
        {
            RegisterTemplateCallback(CareerStartTemplateId.OnYourOwn);
            RegisterTemplateCallback(CareerStartTemplateId.AtRockBottom);
            RegisterTemplateCallback(CareerStartTemplateId.PrivilegedStart);
            RegisterTemplateCallback(CareerStartTemplateId.OneMemeWonder);
            RegisterTemplateCallback(CareerStartTemplateId.BasementGenius);
            RegisterTemplateCallback(CareerStartTemplateId.FormerGroupMember);
            RegisterTemplateCallback(CareerStartTemplateId.Protege);
        }

        private void RegisterTemplateCallback(CareerStartTemplateId templateId)
        {
            if (!_templateButtons.TryGetValue(templateId, out Button button) || button == null)
            {
                return;
            }

            button.clicked += () => _controller.HandleStartTemplateSelected(templateId);
        }

        private void ApplyStaticLocalizedText()
        {
            if (_document == null)
            {
                return;
            }

            _heroStatusLabel.text = GetText(GameLocalizationKeys.UiShell.SplashPreparingStage);
            _subtitleLabel.text = GetText(GameLocalizationKeys.UiShell.MainMenuSubtitle);
            _templateScreenTitleLabel.text = GetText(GameLocalizationKeys.UiShell.TemplateScreenTitle);
            _templateScreenSubtitleLabel.text = GetText(GameLocalizationKeys.UiShell.TemplateScreenSubtitle);
            _hudTitleLabel.text = GetText(GameLocalizationKeys.UiShell.HudTitle);
            _hudBodyLabel.text = GetText(GameLocalizationKeys.UiShell.HudBody);
            _hudOpportunityBodyLabel.text = GetText(GameLocalizationKeys.UiShell.HudOpportunityBody);
            VisualElement root = _document.rootVisualElement;
            root.Q<Label>("hud-location-label").text = GetText(GameLocalizationKeys.UiShell.HudLocation);
            root.Q<Label>("hud-energy-label").text = GetText(GameLocalizationKeys.UiShell.HudEnergy);
            root.Q<Label>("hud-satiety-label").text = GetText(GameLocalizationKeys.UiShell.HudSatiety);
            root.Q<Label>("hud-motivation-label").text = GetText(GameLocalizationKeys.UiShell.HudMotivation);
            root.Q<Label>("hud-money-label").text = GetText(GameLocalizationKeys.UiShell.HudMoney);
            root.Q<Label>("hud-fans-label").text = GetText(GameLocalizationKeys.UiShell.HudFans);
            root.Q<Label>("hud-hype-label").text = GetText(GameLocalizationKeys.UiShell.HudHype);
            root.Q<Label>("hud-opportunity-label").text = GetText(GameLocalizationKeys.UiShell.HudOpportunity);
            root.Q<Label>("hud-nav-home").text = GetText(GameLocalizationKeys.UiShell.HudNavHome);
            root.Q<Label>("hud-nav-map").text = GetText(GameLocalizationKeys.UiShell.HudNavMap);
            root.Q<Label>("hud-nav-career").text = GetText(GameLocalizationKeys.UiShell.HudNavCareer);
            root.Q<Label>("hud-nav-inbox").text = GetText(GameLocalizationKeys.UiHome.HudNavInbox);
            root.Q<Label>("hud-conditions-status-label").text = GetText(GameLocalizationKeys.UiHome.HudConditionsStatus);
            _newCareerButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuNewCareer);
            _continueButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuContinue);
            _quitButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuQuit);
            _menuButton.text = GetText(GameLocalizationKeys.UiShell.HudMenu);
            _hudActionButton.text = GetText(GameLocalizationKeys.UiHome.HudActionPrimary);
            _startTemplateBackButton.text = GetText(GameLocalizationKeys.UiShell.TemplateBack);
            _startTemplateConfirmButton.text = GetText(GameLocalizationKeys.UiShell.TemplateConfirm);

            Label[] brandTitles = _document.rootVisualElement.Query<Label>(className: "brand-title").ToList().ToArray();
            for (int index = 0; index < brandTitles.Length; index++)
            {
                brandTitles[index].text = GetText(GameLocalizationKeys.UiShell.BrandTitle);
            }

            foreach (KeyValuePair<CareerStartTemplateId, Button> pair in _templateButtons)
            {
                AppShellStartTemplateDefinition definition = AppShellStartTemplateCatalog.Get(pair.Key);
                pair.Value.text = GetText(definition.TitleKey);
            }
        }

        private string GetText(LocalizationKey key)
        {
            return _localizationService?.Get(key) ?? string.Empty;
        }

        private string GetText(LocalizationKey key, params LocalizationArgument[] arguments)
        {
            return _localizationService?.Get(key, arguments) ?? string.Empty;
        }

        private static string FormatDate(GameState state)
        {
            var date = state.Calendar.CurrentDate;
            return $"{date.Day:D2}.{date.Month:D2}\n{date.Hour:D2}:00";
        }

        private static string FormatPercent(long current, long maximum)
        {
            if (maximum <= 0)
            {
                return "0%";
            }

            long percent = current * 100 / maximum;
            return percent.ToString(CultureInfo.InvariantCulture) + "%";
        }

        private static string FormatMoney(long minorUnits)
        {
            decimal majorUnits = minorUnits / 100m;
            return majorUnits.ToString("N0", CultureInfo.InvariantCulture) + " ₽";
        }
    }
}
