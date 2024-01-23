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
        [SerializeField] private Dictionary<OverlayWindowType, CanvasUIElement> _windows;
        [SerializeField] private OverlayWindowType _startWindow;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private OverlayWindowType _activeWindowType;

        private readonly Stack<IUIElement> _windowHistory = new();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var overlayWindow in _windows.Values)
                overlayWindow.Initialize();

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
            return _windows.GetValueOrDefault(windowType);
        }

        private OverlayWindowType GetOverlayWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _windows)
            {
                if (windowData.Value == window)
                    return windowData.Key;
            }
            
            return OverlayWindowType.None;
        }
    }
}
