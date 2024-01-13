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
        [SerializeField] private Dictionary<OverlayWindowType, CanvasUIElement> overlaysWindows;
        [SerializeField] private OverlayWindowType startOverlayWindow;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private OverlayWindowType _activeOverlayWindowType = OverlayWindowType.None;

        private readonly Stack<IUIElement> _windowHistory = new();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var overlayWindow in overlaysWindows.Values)
                overlayWindow.Initialize();

            uiMessageBroker
                .Receive<OverlayWindowControlMessage>()
                .Subscribe(msg => ManageOverlayWindowControl(msg.Type))
                .AddTo(disposables);
        }

        private void ShowOverlayWindow(OverlayWindowType overlayWindowType, bool pause = true)
        {
            if (overlayWindowType == _activeOverlayWindowType) return;

            var newOverlayWindow = GetOverlayWindow(overlayWindowType);
            if (newOverlayWindow is null) return;
            Activate();

            _windowHistory.Push(GetOverlayWindow(_activeOverlayWindowType));

            CloseCurrentOverlayWindow();
            newOverlayWindow.Show();
            _activeOverlayWindowType = overlayWindowType;

            overlayBlackout.Show();
        }

        private void CloseCurrentOverlayWindow()
        {
            var currentOverlayWindow = GetOverlayWindow(_activeOverlayWindowType);
            currentOverlayWindow?.Hide();

            _activeOverlayWindowType = OverlayWindowType.None;
            overlayBlackout.Hide();
        }

        private void ManageOverlayWindowControl(OverlayWindowType windowType)
        {
            switch (windowType)
            {
                case OverlayWindowType.Previous:
                    var prevUIElement = _windowHistory.TryPop(out IUIElement result);
                    if (prevUIElement is false)
                    {
                        _windowHistory.Clear();
                        CloseCurrentOverlayWindow();

                        Deactivate();
                        return;
                    }

                    var prevOverlayWindow = result as CanvasUIElement;
                    var prevWindowType = GetOverlayWindowType(prevOverlayWindow);
                    ShowOverlayWindow(prevWindowType);
                    break;
                
                case OverlayWindowType.None:
                    _windowHistory.Clear();
                    CloseCurrentOverlayWindow();
                    Deactivate();
                    break;
                
                default:
                    ShowOverlayWindow(windowType);
                    break;
            }
        }

        private CanvasUIElement GetOverlayWindow(OverlayWindowType overlayWindowType)
        {
            return overlaysWindows.GetValueOrDefault(overlayWindowType);
        }

        private OverlayWindowType GetOverlayWindowType(CanvasUIElement window)
        {
            foreach (var windowData in overlaysWindows)
            {
                if (windowData.Value == window)
                    return windowData.Key;
            }
            
            return OverlayWindowType.None;
        }
    }
}
