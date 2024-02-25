using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public class WindowContainer : UIElementContainer
    {
        [DictionaryDrawerSettings(KeyLabel = "Window type", ValueLabel = "Window settings")]
        [SerializeField] private Dictionary<WindowType, CanvasUIElement> _windows;
        [SerializeField] private WindowType _startWindow;

        private WindowType _activeWindow;
        private readonly Stack<WindowType> _windowsHistory = new();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var window in _windows.Values)
                window.Initialize();
            
            UIMessageBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type, msg.Context))
                .AddTo(disposables);
            
            HideAnyWindow();
            ManageWindowControl(_startWindow, new object());
        }

        private void ManageWindowControl(WindowType windowType, object ctx)
        {
            switch (windowType)
            {
                case WindowType.Previous:
                    if (_windowsHistory.TryPop(out var prevType))
                    {
                        ChangeWindow(prevType, ctx);   
                    }
                    break;
                
                case WindowType.None:
                    _windowsHistory.Clear();
                    break;
                
                default:
                    ChangeWindow(windowType, ctx);
                    break;
            }
        }
        
        private void ChangeWindow(WindowType windowType, object ctx)
        {
            if (windowType == _activeWindow) 
                return;
            
            var window = GetWindow(windowType);
            if (window == null) 
                return;
            
            HideCurrentWindow();
            window.Show(ctx);
            
            _windowsHistory.Push(_activeWindow);
            _activeWindow = windowType;
        }

        private CanvasUIElement GetWindow(WindowType windowType)
        {
            return _windows.GetValueOrDefault(windowType);
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
    }
}
