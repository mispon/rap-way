using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Interfaces;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Utils;

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

        private MementoHistory<IUIElement> _windowHistory;

        public override void Initialize()
        {
            base.Initialize();

            _windowHistory = new MementoHistory<IUIElement>();

            SetupListeners();

            uiMessageBroker
                .Receive<OverlayWindowControlMessage>()
                .Subscribe(msg => ManageOverlayWindowControl(msg.OverlayWindowType))
                .AddTo(disposables);

            foreach (var overlayWindow in _overlaysWindows.Values)
                overlayWindow.Initialize();
        }

        private void ShowOverlayWindow(OverlayWindowType overlayWindowType, bool pause = true)
        {
            if (overlayWindowType == _activeOverlayWindowType) return;

            var newOverlayWindow = GetOverlayWindow(overlayWindowType);
            if (newOverlayWindow is null) return;
            Activate();

            _windowHistory.AddNewElement(newOverlayWindow);

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
                    var prevUIElement = _windowHistory.GetPreviousElement();
                    if (prevUIElement is null)
                    {
                        _windowHistory.ClearHistory();
                        CloseCurrentOverlayWindow();

                        Deactivate();
                        return;
                    }

                    var prevOverlayWindow = prevUIElement as CanvasUIElement;
                    var prevWindowType = GetOverlayWindowType(prevOverlayWindow);
                    ShowOverlayWindow(prevWindowType);
                    break;
                case OverlayWindowType.None:
                    _windowHistory.ClearHistory();
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
            return
                _overlaysWindows.ContainsKey(overlayWindowType) ?
                _overlaysWindows[overlayWindowType] :
                null;
        }

        private OverlayWindowType GetOverlayWindowType(CanvasUIElement window)
        {
            foreach (var windowData in _overlaysWindows)
                if (windowData.Value == window)
                    return windowData.Key;
            return OverlayWindowType.None;
        }

        protected override void SetupListeners()
        {
            
        }
    }
}
