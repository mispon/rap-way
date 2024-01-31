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
        [SerializeField] private Dictionary<WindowType, TutorialWindow> tutorials;
        [SerializeField, ChildGameObjectsOnly] private OverlayBlackout overlayBlackout;
        
        private TutorialWindow _activeTutorial;
        private const string SAVE_KEY_FIRST_TUTORIAL = "FirstTutorial";

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (var tutorial in tutorials.Values)
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
            
            var newTutorial = GetTutorial(windowType);
            
            if (newTutorial is null) return;
            if (newTutorial.IsFirstTutorial == false) return;
            if (GetIndexTutorial(windowType, out var indexTutorial) == false) return;

            Activate();
            CloseCurrentTutorial();
            
            newTutorial.Show();
            newTutorial.ShowFirstTutorial(indexTutorial);

            _activeTutorial = newTutorial;

            overlayBlackout.Show();
        }
        
        private bool GetIndexTutorial(WindowType windowType, out int nextIndex)
        {
            string key = $"{SAVE_KEY_FIRST_TUTORIAL}{windowType}";

            nextIndex = 0;

            if (PlayerPrefs.HasKey(key))
            {
                var index = PlayerPrefs.GetInt(key);
                nextIndex = ++index;
                
                if (nextIndex >= GetTutorial(windowType).FirstTutorialCount) return false;

                PlayerPrefs.SetInt(key, nextIndex);
                return true;
            }
            else
            { 
                PlayerPrefs.SetInt(key, nextIndex);
                return true;
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
            return tutorials.GetValueOrDefault(windowType);
        }

        protected override void Deactivate()
        {
            base.Deactivate();
            CloseCurrentTutorial();
        }
    }
}
