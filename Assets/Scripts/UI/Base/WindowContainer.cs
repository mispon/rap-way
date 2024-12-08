using System;
using System.Collections.Generic;
using System.Linq;
using Core.Ads;
using Core.PropertyAttributes;
using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    [Serializable]
    public class WindowTuple
    {
        public WindowType      Type;
        public CanvasUIElement Value;
    }

    public class WindowContainer : UIElementContainer
    {
        [ArrayElementTitle(new[] {"Type"})]
        [SerializeField] private WindowTuple[] windows;
        [SerializeField] private WindowType startWindow;

        private          WindowType        _activeWindow;
        private readonly Stack<WindowType> _windowsHistory = new();

        public override void Initialize()
        {
            base.Initialize();

            foreach (var window in windows)
            {
                window.Value.Initialize();
            }

            MsgBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ManageWindowControl(msg.Type, msg.Context))
                .AddTo(disposables);

            HideAnyWindow();
            ManageWindowControl(startWindow, new object());
        }

        private void ManageWindowControl(WindowType windowType, object ctx)
        {
#if UNITY_ANDROID
            CasAdsManager.Instance.ShowInterstitial();
#elif UNITY_WEBGL
            YandexAdsManager.Instance.ShowInterstitial();
#endif

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
            {
                return;
            }

            var window = GetWindow(windowType);
            if (window == null)
            {
                return;
            }

            HideCurrentWindow();

            _windowsHistory.Push(_activeWindow);
            _activeWindow = windowType;

            window.Show(ctx);
        }

        private CanvasUIElement GetWindow(WindowType windowType)
        {
            return windows.FirstOrDefault(e => e.Type == windowType)?.Value;
        }

        private void HideCurrentWindow()
        {
            GetWindow(_activeWindow)?.Hide();
        }

        private void HideAnyWindow()
        {
            HideCurrentWindow();
            _activeWindow = WindowType.None;
        }
    }
}