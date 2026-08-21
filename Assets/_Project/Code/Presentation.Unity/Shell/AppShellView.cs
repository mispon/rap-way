using System;
using System.Collections.Generic;
using RapWay.Application.Session;
using RapWay.Presentation.Unity.Localization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace RapWay.Presentation.Unity.Shell
{
    public sealed class AppShellView : MonoBehaviour
    {
        private const string VisualTreeResourcePath = "UI/AppShell";
        private const string StyleSheetResourcePath = "UI/AppShell";
        private const float BaseLandscapeWidth = 1920f;
        private const float BasePortraitWidth = 1080f;

        private UIDocument _document;
        private AppShellController _controller;
        private IGameLocalizationService _localizationService;
        private VisualElement _safeAreaFrame;
        private VisualElement _modalOverlay;
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
        private Label _modalTitle;
        private Label _modalBody;
        private Button _modalPrimaryButton;
        private Button _modalSecondaryButton;
        private VisualElement _heroCard;
        private VisualElement _menuCard;
        private Label _heroStatusLabel;
        private Label _mainMenuTitleLabel;
        private Label _subtitleLabel;
        private Label _templateScreenTitleLabel;
        private Label _templateScreenSubtitleLabel;
        private Label _startTemplateTitleLabel;
        private Label _startTemplateTaglineLabel;
        private Label _startTemplateSummaryLabel;
        private Label _startTemplateEmphasisLabel;
        private Label _hudTitleLabel;
        private Label _hudBodyLabel;
        private Button _newCareerButton;
        private Button _quitButton;
        private Dictionary<CareerStartTemplateId, Button> _templateButtons;
        private Rect _lastSafeArea;
        private Vector2Int _lastResolution;

        public static AppShellView Create()
        {
            GameObject shellObject = new("Stage4AppShell");
            DontDestroyOnLoad(shellObject);
            return shellObject.AddComponent<AppShellView>();
        }

        public void Initialize(AppShellController controller, IGameLocalizationService localizationService)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            EnsureEventSystem();
            EnsureDocument();
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

            _modalOverlay.style.display = ToDisplayStyle(state.ActiveModal != AppShellModal.None);
            ConfigureModal(state);
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

        private void EnsureDocument()
        {
            if (_document != null)
            {
                return;
            }

            _document = gameObject.AddComponent<UIDocument>();
            _document.panelSettings = CreatePanelSettings();
            _document.sortingOrder = 100;

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>(VisualTreeResourcePath);
            StyleSheet styleSheet = Resources.Load<StyleSheet>(StyleSheetResourcePath);
            if (visualTree == null)
            {
                throw new InvalidOperationException($"Visual tree '{VisualTreeResourcePath}' was not found.");
            }

            VisualElement root = _document.rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;
            root.style.width = Length.Percent(100);
            root.style.height = Length.Percent(100);
            if (styleSheet != null)
            {
                root.styleSheets.Add(styleSheet);
            }

            visualTree.CloneTree(root);
        }

        private static PanelSettings CreatePanelSettings()
        {
            PanelSettings panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            panelSettings.scale = CalculatePanelScale();
            panelSettings.sortingOrder = 100;

            return panelSettings;
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
            _modalOverlay = root.Q<VisualElement>("modal-overlay");
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
            _modalTitle = root.Q<Label>("modal-title");
            _modalBody = root.Q<Label>("modal-body");
            _modalPrimaryButton = root.Q<Button>("modal-primary-button");
            _modalSecondaryButton = root.Q<Button>("modal-secondary-button");
            _heroCard = root.Q<VisualElement>(className: "hero-card");
            _menuCard = root.Q<VisualElement>(className: "menu-card");
            _heroStatusLabel = root.Q<Label>(className: "status-label--hero");
            _subtitleLabel = root.Q<Label>(className: "subtitle-label");
            _templateScreenTitleLabel = root.Q<Label>(className: "template-screen-title");
            _templateScreenSubtitleLabel = root.Q<Label>(className: "template-screen-subtitle");
            _startTemplateTitleLabel = root.Q<Label>("template-detail-title");
            _startTemplateTaglineLabel = root.Q<Label>("template-detail-tagline");
            _startTemplateSummaryLabel = root.Q<Label>("template-detail-summary");
            _startTemplateEmphasisLabel = root.Q<Label>("template-detail-emphasis");
            _hudTitleLabel = root.Q<Label>(className: "hud-title");
            _hudBodyLabel = root.Q<Label>(className: "hud-body");

            _newCareerButton = root.Q<Button>("new-career-button");
            _quitButton = root.Q<Button>("quit-button");
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
            Label[] brandTitles = root.Query<Label>(className: "brand-title").ToList().ToArray();
            if (brandTitles.Length > 1)
            {
                _mainMenuTitleLabel = brandTitles[1];
            }

            _newCareerButton.clicked += _controller.HandleNewCareerRequested;
            _continueButton.clicked += _controller.HandleContinueRequested;
            _quitButton.clicked += _controller.HandleBackAction;
            _menuButton.clicked += _controller.HandleBackAction;
            _startTemplateBackButton.clicked += _controller.HandleBackAction;
            _startTemplateConfirmButton.clicked += _controller.HandleStartTemplateConfirmed;
            _modalSecondaryButton.clicked += _controller.HandleModalDismiss;

            RegisterTemplateCallbacks();
            ApplyFallbackStyling(root, _newCareerButton, _quitButton);
            RefreshSafeArea(force: true);
        }

        private void ConfigureModal(AppShellState state)
        {
            if (state.ActiveModal == AppShellModal.None)
            {
                return;
            }

            switch (state.ActiveModal)
            {
                case AppShellModal.ExitConfirmation:
                    _modalTitle.text = GetText(GameLocalizationKeys.UiShell.ModalExitTitle);
                    _modalBody.text = GetText(GameLocalizationKeys.UiShell.ModalExitBody);
                    _modalPrimaryButton.text = GetText(GameLocalizationKeys.UiShell.ModalExitConfirm);
                    _modalPrimaryButton.clicked -= _controller.HandleReturnToMenuRequested;
                    _modalPrimaryButton.clicked -= _controller.HandleQuitConfirmed;
                    _modalPrimaryButton.clicked += _controller.HandleQuitConfirmed;
                    break;

                case AppShellModal.SessionMenu:
                    _modalTitle.text = GetText(GameLocalizationKeys.UiShell.ModalSessionTitle);
                    _modalBody.text = GetText(GameLocalizationKeys.UiShell.ModalSessionBody);
                    _modalPrimaryButton.text = GetText(GameLocalizationKeys.UiShell.ModalSessionConfirm);
                    _modalPrimaryButton.clicked -= _controller.HandleQuitConfirmed;
                    _modalPrimaryButton.clicked -= _controller.HandleReturnToMenuRequested;
                    _modalPrimaryButton.clicked += _controller.HandleReturnToMenuRequested;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
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
            Rect safeArea = Screen.safeArea;
            Vector2Int resolution = new(Screen.width, Screen.height);
            if (!force && safeArea == _lastSafeArea && resolution == _lastResolution)
            {
                return;
            }

            _lastSafeArea = safeArea;
            _lastResolution = resolution;

            if (_document?.panelSettings != null)
            {
                _document.panelSettings.scale = CalculatePanelScale();
            }

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
                ApplyTemplateButtonStyle(pair.Value, isSelected);
            }
        }

        private void ApplyFallbackStyling(VisualElement root, Button newCareerButton, Button quitButton)
        {
            root.style.backgroundColor = new StyleColor(new Color(0.0627451f, 0.07058824f, 0.09411765f, 1f));

            _safeAreaFrame.style.flexDirection = FlexDirection.Column;
            _safeAreaFrame.style.justifyContent = Justify.Center;
            _safeAreaFrame.style.alignItems = Align.Center;

            _splashScreen.style.justifyContent = Justify.Center;
            _splashScreen.style.alignItems = Align.Center;
            _mainMenuScreen.style.justifyContent = Justify.Center;
            _mainMenuScreen.style.alignItems = Align.Center;
            _newCareerTemplateScreen.style.justifyContent = Justify.Center;
            _newCareerTemplateScreen.style.alignItems = Align.Center;

            ApplyCardStyle(_heroCard);
            ApplyCardStyle(_menuCard);
            ApplyCardStyle(_newCareerTemplateScreen.Q<VisualElement>(className: "template-selection-card"));
            ApplyCardStyle(_hudScreen.Q<VisualElement>(className: "hud-card"));
            ApplyCardStyle(_modalOverlay.Q<VisualElement>(className: "modal-card"));

            ApplyLabelStyle(_heroStatusLabel, 26f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_mainMenuTitleLabel, 72f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_subtitleLabel, 22f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_startTemplateTitleLabel, 34f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_startTemplateTaglineLabel, 20f, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_startTemplateSummaryLabel, 16f, new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_startTemplateEmphasisLabel, 15f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_mainMenuStatusLabel, 18f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_newCareerStatusLabel, 18f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_modalTitle, 22f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_modalBody, 16f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_hudTitleLabel, 24f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_hudBodyLabel, 14f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);
            _startTemplateSummaryLabel.style.unityTextAlign = TextAnchor.UpperLeft;
            _startTemplateEmphasisLabel.style.unityTextAlign = TextAnchor.UpperLeft;

            ApplyButtonStyle(newCareerButton, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), new Color(1f, 0.972549f, 0.95686275f, 1f));
            ApplyButtonStyle(_continueButton, new Color(0.16078432f, 0.1882353f, 0.25882354f, 1f), new Color(0.95686275f, 0.96862745f, 1f, 1f));
            ApplyButtonStyle(quitButton, new Color(1f, 1f, 1f, 0.04f), new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f));
            ApplyButtonStyle(_menuButton, new Color(0.16078432f, 0.1882353f, 0.25882354f, 1f), new Color(0.95686275f, 0.96862745f, 1f, 1f));
            ApplyButtonStyle(_startTemplateBackButton, new Color(1f, 1f, 1f, 0.04f), new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f));
            ApplyButtonStyle(_startTemplateConfirmButton, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), new Color(1f, 0.972549f, 0.95686275f, 1f));
            ApplyButtonStyle(_modalPrimaryButton, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), new Color(1f, 0.972549f, 0.95686275f, 1f));
            ApplyButtonStyle(_modalSecondaryButton, new Color(1f, 1f, 1f, 0.04f), new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f));
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
            _newCareerButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuNewCareer);
            _continueButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuContinue);
            _quitButton.text = GetText(GameLocalizationKeys.UiShell.MainMenuQuit);
            _menuButton.text = GetText(GameLocalizationKeys.UiShell.HudMenu);
            _startTemplateBackButton.text = GetText(GameLocalizationKeys.UiShell.TemplateBack);
            _startTemplateConfirmButton.text = GetText(GameLocalizationKeys.UiShell.TemplateConfirm);
            _modalSecondaryButton.text = GetText(GameLocalizationKeys.UiShell.ModalClose);

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

        private static void ApplyCardStyle(VisualElement card)
        {
            if (card == null)
            {
                return;
            }

            card.style.width = Length.Percent(100);
            card.style.maxWidth = 520f;
            card.style.paddingLeft = 32f;
            card.style.paddingRight = 32f;
            card.style.paddingTop = 32f;
            card.style.paddingBottom = 32f;
            card.style.backgroundColor = new StyleColor(new Color(0.09411765f, 0.10980392f, 0.14901961f, 0.92f));
        }

        private static void ApplyLabelStyle(Label label, float fontSize, Color color, FontStyle fontStyle)
        {
            if (label == null)
            {
                return;
            }

            label.style.fontSize = fontSize;
            label.style.color = new StyleColor(color);
            label.style.unityFontStyleAndWeight = fontStyle;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }

        private static void ApplyButtonStyle(Button button, Color backgroundColor, Color textColor)
        {
            if (button == null)
            {
                return;
            }

            button.style.width = Length.Percent(100);
            button.style.minHeight = 60f;
            button.style.backgroundColor = new StyleColor(backgroundColor);
            button.style.color = new StyleColor(textColor);
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            button.style.fontSize = 20f;

            Label buttonLabel = button.Q<Label>();
            if (buttonLabel != null)
            {
                buttonLabel.style.color = new StyleColor(textColor);
                buttonLabel.style.fontSize = 20f;
                buttonLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                buttonLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            }
        }

        private static void ApplyTemplateButtonStyle(Button button, bool isSelected)
        {
            if (button == null)
            {
                return;
            }

            Color backgroundColor = isSelected
                ? new Color(0.9529412f, 0.43137255f, 0.25490198f, 0.2f)
                : new Color(0.16078432f, 0.1882353f, 0.25882354f, 0.85f);
            Color borderColor = isSelected
                ? new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f)
                : new Color(0.31764707f, 0.3529412f, 0.44313726f, 1f);
            Color textColor = isSelected
                ? new Color(1f, 0.972549f, 0.95686275f, 1f)
                : new Color(0.95686275f, 0.96862745f, 1f, 1f);

            button.style.backgroundColor = new StyleColor(backgroundColor);
            button.style.borderLeftColor = borderColor;
            button.style.borderRightColor = borderColor;
            button.style.borderTopColor = borderColor;
            button.style.borderBottomColor = borderColor;
            button.style.borderLeftWidth = 2f;
            button.style.borderRightWidth = 2f;
            button.style.borderTopWidth = 2f;
            button.style.borderBottomWidth = 2f;
            button.style.color = new StyleColor(textColor);

            Label buttonLabel = button.Q<Label>();
            if (buttonLabel != null)
            {
                buttonLabel.style.color = new StyleColor(textColor);
            }
        }

        private static float CalculatePanelScale()
        {
            float referenceWidth = Screen.height > Screen.width ? BasePortraitWidth : BaseLandscapeWidth;
            float safeScreenWidth = Mathf.Max(1f, Screen.width);
            return Mathf.Max(1f, safeScreenWidth / referenceWidth);
        }
    }
}
