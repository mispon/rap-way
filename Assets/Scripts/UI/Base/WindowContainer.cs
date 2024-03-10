using System.Collections.Generic;
using MessageBroker;
using MessageBroker.Messages.UI;
using Sirenix.OdinInspector;
using UI.Enums;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public class WindowContainer : UIElementContainer
    {
        [DictionaryDrawerSettings(KeyLabel = "Window type", ValueLabel = "Window settings")]
        [SerializeField] private Dictionary<WindowType, CanvasUIElement> windows;
        [SerializeField] private WindowType startWindow;

        private WindowType _activeWindow;
        private readonly Stack<WindowType> _windowsHistory = new();

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var window in windows.Values)
                window.Initialize();
            
            MsgBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type, msg.Context))
                .AddTo(disposables);
            
            HideAnyWindow();
            ManageWindowControl(startWindow, new object());
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
            
            _windowsHistory.Push(_activeWindow);
            _activeWindow = windowType;
            
            window.Show(ctx);
        }

        private CanvasUIElement GetWindow(WindowType windowType)
        {
            return windows.GetValueOrDefault(windowType);
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
