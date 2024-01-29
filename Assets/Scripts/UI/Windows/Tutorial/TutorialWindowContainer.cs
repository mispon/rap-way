using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace UI.Windows.Tutorial
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

            UIMessageBroker.Instance
                .Receive<TutorialWindowControlMessage>()
                .Subscribe(msg => ShowTutorial(msg.Type))
                .AddTo(disposables);
            
            UIMessageBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(msg => ShowFirstTutorial(msg.Type))
                .AddTo(disposables);
        }

        private void ShowTutorial(WindowType windowType)
        {
            if (windowType == WindowType.None) 
                Deactivate();
            
            var newTutorial = GetTutorial(windowType);
            
            if (newTutorial == null || newTutorial == _activeTutorial || !newTutorial.IsContainsTutorials) 
                return;

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

            if (IsTutorialShowed(windowType)) return;
            
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
        
        private static bool IsTutorialShowed(WindowType windowType)
        {
            string key = $"{SAVE_KEY_FIRST_TUTORIAL}{windowType}";
            
            if (PlayerPrefs.HasKey(key)) 
                return true;
            
            PlayerPrefs.SetInt(key, 1);
            return false;
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
