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
        [SerializeField]
        private Dictionary<OverlayWindowType, CanvasUIElement> _overlaysWindows;

        private OverlayWindowType _activeOverlayWindowType = OverlayWindowType.None;

        [SerializeField]
        private OverlayWindowType _startOverlayWindow;

        [ChildGameObjectsOnly]
        [SerializeField]
        private OverlayBlackout _overlayBlackout;

        private readonly Stack<IUIElement> _windowHistory = new Stack<IUIElement>();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var overlayWindow in _overlaysWindows.Values)
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

            _windowHistory.Push(newOverlayWindow);

            CloseCurrentOverlayWindow();
            //if (pause == true) TimeCustom.Pause();
            newOverlayWindow.Show();
            _activeOverlayWindowType = overlayWindowType;

            _overlayBlackout.Show();
        }

        private void CloseCurrentOverlayWindow()
        {
            var currentOverlayWindow = GetOverlayWindow(_activeOverlayWindowType);
            currentOverlayWindow?.Hide();

            _activeOverlayWindowType = OverlayWindowType.None;
            _overlayBlackout.Hide();
            //TimeCustom.Play();
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
            return _overlaysWindows.GetValueOrDefault(overlayWindowType);
        }

        private OverlayWindowType GetOverlayWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _overlaysWindows)
                if (windowData.Value == window)
                    return windowData.Key;
            return OverlayWindowType.None;
        }
    }
}
