using System;
using Cysharp.Threading.Tasks;
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
        private VisualElement _safeAreaFrame;
        private VisualElement _modalOverlay;
        private VisualElement _splashScreen;
        private VisualElement _mainMenuScreen;
        private VisualElement _hudScreen;
        private Label _statusLabel;
        private Button _continueButton;
        private Button _menuButton;
        private Label _modalTitle;
        private Label _modalBody;
        private Button _modalPrimaryButton;
        private Button _modalSecondaryButton;
        private VisualElement _heroCard;
        private VisualElement _menuCard;
        private Label _heroStatusLabel;
        private Label _mainMenuTitleLabel;
        private Label _subtitleLabel;
        private Label _hudTitleLabel;
        private Label _hudBodyLabel;
        private Rect _lastSafeArea;
        private Vector2Int _lastResolution;

        public static AppShellView Create()
        {
            GameObject shellObject = new("Stage4AppShell");
            DontDestroyOnLoad(shellObject);
            return shellObject.AddComponent<AppShellView>();
        }

        public void Initialize(AppShellController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
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
            _hudScreen.style.display = ToDisplayStyle(state.ActiveScreen == AppShellScreen.Hud);

            _continueButton.SetEnabled(state.CanContinue && !state.IsBusy);
            _menuButton.SetEnabled(!state.IsBusy);
            _statusLabel.text = state.StatusText ?? string.Empty;

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
            _hudScreen = root.Q<VisualElement>("screen-hud");
            _statusLabel = root.Q<Label>("status-label");
            _continueButton = root.Q<Button>("continue-button");
            _menuButton = root.Q<Button>("hud-menu-button");
            _modalTitle = root.Q<Label>("modal-title");
            _modalBody = root.Q<Label>("modal-body");
            _modalPrimaryButton = root.Q<Button>("modal-primary-button");
            _modalSecondaryButton = root.Q<Button>("modal-secondary-button");
            _heroCard = root.Q<VisualElement>(className: "hero-card");
            _menuCard = root.Q<VisualElement>(className: "menu-card");
            _heroStatusLabel = root.Q<Label>(className: "status-label--hero");
            _subtitleLabel = root.Q<Label>(className: "subtitle-label");
            _hudTitleLabel = root.Q<Label>(className: "hud-title");
            _hudBodyLabel = root.Q<Label>(className: "hud-body");

            Button newCareerButton = root.Q<Button>("new-career-button");
            Button quitButton = root.Q<Button>("quit-button");
            Label[] brandTitles = root.Query<Label>(className: "brand-title").ToList().ToArray();
            if (brandTitles.Length > 1)
            {
                _mainMenuTitleLabel = brandTitles[1];
            }

            newCareerButton.clicked += _controller.HandleNewCareerRequested;
            _continueButton.clicked += _controller.HandleContinueRequested;
            quitButton.clicked += _controller.HandleBackAction;
            _menuButton.clicked += _controller.HandleBackAction;
            _modalSecondaryButton.clicked += _controller.HandleModalDismiss;

            ApplyFallbackStyling(root, newCareerButton, quitButton);
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
                    _modalTitle.text = "Exit Rap Way?";
                    _modalBody.text = "Close the app now.";
                    _modalPrimaryButton.text = "Exit";
                    _modalPrimaryButton.clicked -= _controller.HandleReturnToMenuRequested;
                    _modalPrimaryButton.clicked -= _controller.HandleQuitConfirmed;
                    _modalPrimaryButton.clicked += _controller.HandleQuitConfirmed;
                    break;

                case AppShellModal.SessionMenu:
                    _modalTitle.text = "Session Menu";
                    _modalBody.text = "Return to the main menu scene.";
                    _modalPrimaryButton.text = "Main Menu";
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

            ApplyCardStyle(_heroCard);
            ApplyCardStyle(_menuCard);
            ApplyCardStyle(_hudScreen.Q<VisualElement>(className: "hud-card"));
            ApplyCardStyle(_modalOverlay.Q<VisualElement>(className: "modal-card"));

            ApplyLabelStyle(_heroStatusLabel, 26f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_mainMenuTitleLabel, 72f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_subtitleLabel, 22f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_statusLabel, 18f, new Color(0.7294118f, 0.76862746f, 0.8627451f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_modalTitle, 22f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_modalBody, 16f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);
            ApplyLabelStyle(_hudTitleLabel, 24f, new Color(0.95686275f, 0.92156863f, 0.8392157f, 1f), FontStyle.Bold);
            ApplyLabelStyle(_hudBodyLabel, 14f, new Color(0.654902f, 0.6901961f, 0.7764706f, 1f), FontStyle.Normal);

            ApplyButtonStyle(newCareerButton, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), new Color(1f, 0.972549f, 0.95686275f, 1f));
            ApplyButtonStyle(_continueButton, new Color(0.16078432f, 0.1882353f, 0.25882354f, 1f), new Color(0.95686275f, 0.96862745f, 1f, 1f));
            ApplyButtonStyle(quitButton, new Color(1f, 1f, 1f, 0.04f), new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f));
            ApplyButtonStyle(_menuButton, new Color(0.16078432f, 0.1882353f, 0.25882354f, 1f), new Color(0.95686275f, 0.96862745f, 1f, 1f));
            ApplyButtonStyle(_modalPrimaryButton, new Color(0.9529412f, 0.43137255f, 0.25490198f, 1f), new Color(1f, 0.972549f, 0.95686275f, 1f));
            ApplyButtonStyle(_modalSecondaryButton, new Color(1f, 1f, 1f, 0.04f), new Color(0.8627451f, 0.8862745f, 0.9411765f, 1f));
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

        private static float CalculatePanelScale()
        {
            float referenceWidth = Screen.height > Screen.width ? BasePortraitWidth : BaseLandscapeWidth;
            float safeScreenWidth = Mathf.Max(1f, Screen.width);
            return Mathf.Max(1f, safeScreenWidth / referenceWidth);
        }
    }
}
