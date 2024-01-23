using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.UI.TutorialWindows
{
    public sealed class TutorialWindowContainer: UIElementContainer
    {
        [SerializeField] private Dictionary<TutorialWindowType, TutorialWindow> _windows;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private TutorialWindowType _activeWindowType;

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var overlayWindow in _windows.Values)
                overlayWindow.Initialize();

            uiMessageBroker
                .Receive<TutorialWindowControlMessage>()
                .Subscribe(msg => ShowWindow(msg.Type))
                .AddTo(disposables);
            
            uiMessageBroker
                .Receive<FirstTutorialControlMessage>()
                .Subscribe(msg => ShowWindow(msg.Type, true))
                .AddTo(disposables);
        }

        private void ShowWindow(TutorialWindowType windowType, bool isFirstTutorial = false)
        {
            if (windowType == _activeWindowType) return;
            
            if (windowType == TutorialWindowType.None)
            {
                CloseCurrentWindow();
                Deactivate();
            }

            var newWindow = GetWindow(windowType);
            if (newWindow is null) return;
            
            if (isFirstTutorial is true && newWindow.IsFirstTutorial is false) return;
            if (isFirstTutorial is false && newWindow.IsContainsTutorials is false) return;
            
            Activate();
            CloseCurrentWindow();
            
            newWindow.Show();
            if (isFirstTutorial is false) newWindow.ShowTutorial();
            else newWindow.ShowFirstTutorial();

            _activeWindowType = windowType;

            overlayBlackout.Show();
        }

        private void CloseCurrentWindow()
        {
            var currentWindow = GetWindow(_activeWindowType);
            currentWindow?.Hide();

            _activeWindowType = TutorialWindowType.None;
            overlayBlackout.Hide();
        }

        private TutorialWindow GetWindow(TutorialWindowType windowType)
        {
            return _windows.GetValueOrDefault(windowType);
        }
    }
}
