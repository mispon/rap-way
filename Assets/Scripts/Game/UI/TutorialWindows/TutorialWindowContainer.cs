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
        [SerializeField] private Dictionary<WindowType, TutorialWindow> _tutorials;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private TutorialWindow _activeTutorial;
        private const string SAVE_KEY_FIRST_TUTORIAL = "FirstTutorial";

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var tutorial in _tutorials.Values)
                tutorial.Initialize();

            uiMessageBroker
                .Receive<TutorialWindowControlMessage>()
                .Subscribe(msg => ShowTutorial(msg.Type))
                .AddTo(disposables);
            
            uiMessageBroker
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ShowFirstTutorial(msg.Type))
                .AddTo(disposables);
        }

        private void ShowTutorial(WindowType windowType)
        {
            if (windowType == WindowType.None) Deactivate();
            
            var newTutorial = GetTutorial(windowType);
            if (newTutorial is null) return;
            if (newTutorial == _activeTutorial) return;
            if (newTutorial.IsContainsTutorials is false) return;

            Activate();
            CloseCurrentTutorial();
            
            newTutorial.Show();
            newTutorial.ShowTutorial();

            _activeTutorial = newTutorial;

            overlayBlackout.Show();
        }
        
        private void ShowFirstTutorial(WindowType windowType)
        {
            if (windowType == WindowType.None) Deactivate();

            if (CheckCompleteFirstTutorial(windowType) is true) return;
            
            var newTutorial = GetTutorial(windowType);
            if (newTutorial is null) return;
            if (newTutorial.IsFirstTutorial is false) return;
            
            Activate();
            CloseCurrentTutorial();
            
            newTutorial.Show();
            newTutorial.ShowFirstTutorial();

            _activeTutorial = newTutorial;

            overlayBlackout.Show();
        }
        
        private bool CheckCompleteFirstTutorial(WindowType windowType)
        {
            if (PlayerPrefs.HasKey($"{SAVE_KEY_FIRST_TUTORIAL}{windowType}") is true) 
                return true;
            else
            {
                PlayerPrefs.SetInt($"{SAVE_KEY_FIRST_TUTORIAL}{windowType}", 1);
                return false;
            }
        }

        private void CloseCurrentTutorial()
        {
            _activeTutorial?.Hide();
            _activeTutorial = null;
            overlayBlackout.Hide();
        }

        private TutorialWindow GetTutorial(WindowType windowType)
        {
            return _tutorials.GetValueOrDefault(windowType);
        }

        protected override void Deactivate()
        {
            base.Deactivate();
            CloseCurrentTutorial();
        }
    }
}
