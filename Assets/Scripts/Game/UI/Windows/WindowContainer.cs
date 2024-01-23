using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Interfaces;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI.Windows
{
    public class WindowContainer : UIElementContainer
    {
        [DictionaryDrawerSettings(KeyLabel = "Window type", ValueLabel = "Window settings")]
        [SerializeField] private Dictionary<WindowType, WindowSettings> _windows;
        [SerializeField] private WindowType _startWindow;

        private WindowType _activeWindow;
        private readonly Stack<IUIElement> _windowHistory = new();
        private const string SAVE_KEY_FIRST_TUTORIAL = "FirstTutorial";

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var window in _windows.Values)
                window.canvas.Initialize();
            
            uiMessageBroker
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type))
                .AddTo(disposables);
            
            HideAnyWindow();
            ManageWindowControl(_startWindow);
        }

        private void ChangeWindow(WindowType windowType)
        {
            if (windowType == _activeWindow) 
                return;
            
            var newWindow = GetWindow(windowType);
            if (newWindow == null) 
                return;
            
            Activate();

            _windowHistory.Push(GetWindow(_activeWindow));
            HideCurrentWindow();
            newWindow.Show();
            
            _activeWindow = windowType;
            
            CheckFirstTutorial(windowType);
        }

        private void ManageWindowControl(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.Previous:
                    var prevUIElement = _windowHistory.TryPop(out IUIElement result);
                    if (prevUIElement is false)
                    {
                        Deactivate();
                        return;
                    }

                    var prevWindow = result as CanvasUIElement;
                    var prevWindowType = GetWindowType(prevWindow);
                    ChangeWindow(prevWindowType);
                    break;
                
                case WindowType.None:
                    _windowHistory.Clear();
                    Deactivate();
                    break;
                
                default:
                    ChangeWindow(windowType);
                    break;
            }
        }

        private CanvasUIElement GetWindow(WindowType windowType)
        {
            return _windows.TryGetValue(windowType, out var window) ? window.canvas : null;
        }

        private WindowType GetWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _windows)
            {
                if (windowData.Value.canvas == window)
                    return windowData.Key;
            }
            
            return WindowType.None;
        }

        private void CheckFirstTutorial(WindowType windowType)
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

        private TutorialWindowType GetFirstTutorial(WindowType windowType)
        {
            return _windows.TryGetValue(windowType, out var window) ? window.tutorialType : TutorialWindowType.None;
        }

        private void HideCurrentWindow()
        {
            var currentWindow = GetWindow(_activeWindow);
            currentWindow?.Hide();
        }

        private void HideAnyWindow()
        {
            HideCurrentWindow();
            _activeWindow = WindowType.None;
        }
        
        protected override void Deactivate()
        {
            base.Deactivate();
            HideAnyWindow();
        }
        
        private struct WindowSettings
        {
            [LabelWidth(100)] public CanvasUIElement canvas;
            [LabelWidth(100)] public TutorialWindowType tutorialType;
        }
    }
}
