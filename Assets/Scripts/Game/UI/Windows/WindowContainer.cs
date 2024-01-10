using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Interfaces;
using Game.UI.Messages;
using UniRx;
using UnityEngine;
using Utils;

namespace Game.UI.Windows
{
    public class WindowContainer : UIElementContainer
    {
        [SerializeField] private Dictionary<WindowType, CanvasUIElement> _windows;
        [SerializeField] private WindowType _startWindow;

        public WindowType StartWindow => _startWindow;

        private WindowType _activeWindow;
        private readonly Stack<IUIElement> _windowHistory = new Stack<IUIElement>();

        public override void Initialize()
        {
            base.Initialize();

            uiMessageBroker
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type))
                .AddTo(disposables);

            foreach (var window in _windows.Values)
                window.Initialize();
        }

        public void ChangeWindow(WindowType windowType)
        {
            if (windowType == _activeWindow && windowType != _startWindow) return;
            
            var newWindow = GetWindow(windowType);
            if (newWindow is null) return;
            Activate();

            _windowHistory.Push(newWindow);
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
            return 
                _windows.ContainsKey(windowType) ? 
                _windows[windowType] : 
                null;
        }

        private WindowType GetWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _windows)
                if (windowData.Value == window)
                    return windowData.Key;
            return WindowType.None;
        }

        private void HideCurrentWindow()
        {
            var currentWindow = GetWindow(_activeWindow);
            currentWindow?.Hide();
        }
        
        public void HideAnyWindow()
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
