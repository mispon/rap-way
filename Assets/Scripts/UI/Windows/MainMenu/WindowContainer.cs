using System.Collections.Generic;
using UI.Base;
using UI.Base.Interfaces;
using UI.Enums;
using Sirenix.OdinInspector;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace UI.Windows.MainMenu
{
    public class WindowContainer : UIElementContainer
    {
        [DictionaryDrawerSettings(KeyLabel = "Window type", ValueLabel = "Window settings")]
        [SerializeField] private Dictionary<WindowType, CanvasUIElement> _windows;
        [SerializeField] private WindowType _startWindow;

        private WindowType _activeWindow;
        private readonly Stack<IUIElement> _windowHistory = new();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var window in _windows.Values)
                window.Initialize();
            
            UIMessageBroker.Instance
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
            return _windows.GetValueOrDefault(windowType);
        }

        private WindowType GetWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _windows)
            {
                if (windowData.Value == window)
                    return windowData.Key;
            }
            
            return WindowType.None;
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
    }
}
