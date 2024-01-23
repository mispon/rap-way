using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Interfaces;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI.OverlayWindows
{
    public sealed class OverlayWindowContainer: UIElementContainer
    {
        [DictionaryDrawerSettings(KeyLabel = "Window type", ValueLabel = "Window settings")]
        [SerializeField] private Dictionary<OverlayWindowType, WindowSettings> _windows;
        [SerializeField] private OverlayWindowType _startWindow;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private OverlayWindowType _activeWindowType;

        private readonly Stack<IUIElement> _windowHistory = new();
        private const string SAVE_KEY_FIRST_TUTORIAL = "FirstTutorial";

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var overlayWindow in _windows.Values)
                overlayWindow.canvas.Initialize();

            uiMessageBroker
                .Receive<OverlayWindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type))
                .AddTo(disposables);
            
            CloseCurrentWindow();
            ManageWindowControl(_startWindow);
        }

        private void ShowWindow(OverlayWindowType windowType, bool pause = true)
        {
            if (windowType == _activeWindowType) return;

            var newWindow = GetWindow(windowType);
            if (newWindow is null) return;
            Activate();

            _windowHistory.Push(GetWindow(_activeWindowType));

            CloseCurrentWindow();
            newWindow.Show();
            _activeWindowType = windowType;

            overlayBlackout.Show();
            
            CheckFirstTutorial(windowType);
        }

        private void CloseCurrentWindow()
        {
            var currentWindow = GetWindow(_activeWindowType);
            currentWindow?.Hide();

            _activeWindowType = OverlayWindowType.None;
            overlayBlackout.Hide();
        }

        private void ManageWindowControl(OverlayWindowType windowType)
        {
            switch (windowType)
            {
                case OverlayWindowType.Previous:
                    var prevUIElement = _windowHistory.TryPop(out IUIElement result);
                    if (prevUIElement is false)
                    {
                        _windowHistory.Clear();
                        CloseCurrentWindow();

                        Deactivate();
                        return;
                    }

                    var prevWindow = result as CanvasUIElement;
                    var prevWindowType = GetOverlayWindowType(prevWindow);
                    ShowWindow(prevWindowType);
                    break;
                
                case OverlayWindowType.None:
                    _windowHistory.Clear();
                    CloseCurrentWindow();
                    Deactivate();
                    break;
                
                default:
                    ShowWindow(windowType);
                    break;
            }
        }
        
        private CanvasUIElement GetWindow(OverlayWindowType windowType)
        {
            return _windows.TryGetValue(windowType, out var window) ? window.canvas : null;
        }

        private OverlayWindowType GetOverlayWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _windows)
            {
                if (windowData.Value.canvas == window)
                    return windowData.Key;
            }
            
            return OverlayWindowType.None;
        }
        
        private void CheckFirstTutorial(OverlayWindowType windowType)
        {
            TutorialWindowType tutorial = GetFirstTutorial(windowType);
            if (tutorial == TutorialWindowType.None) return;
            
            if (PlayerPrefs.HasKey($"{SAVE_KEY_FIRST_TUTORIAL}{windowType}") is true) return;
            
            PlayerPrefs.SetInt($"{SAVE_KEY_FIRST_TUTORIAL}{windowType}", 1);
            uiMessageBroker.Publish(new FirstTutorialControlMessage
            {
                Type = tutorial
            });
        }
        
        private TutorialWindowType GetFirstTutorial(OverlayWindowType windowType)
        {
            return _windows.TryGetValue(windowType, out var window) ? window.tutorialType : TutorialWindowType.None;
        }
        
        private struct WindowSettings
        {
            [LabelWidth(100)] public CanvasUIElement canvas;
            [LabelWidth(100)] public TutorialWindowType tutorialType;
        }
    }
}
