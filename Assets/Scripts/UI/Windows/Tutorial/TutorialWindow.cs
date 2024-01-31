using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.Base;
using UI.Enums;
using UI.MessageBroker;
using UI.MessageBroker.Messages;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Windows.Tutorial
{
    internal class TutorialWindow : CanvasUIElement, IPointerClickHandler
    {
        [SerializeField] private Text _textTutorial;

        [BoxGroup("First tutorial")] [SerializeField] private bool isFirstTutorial;
        
        [BoxGroup("First tutorial"), ShowIf("isFirstTutorial")]
        [SerializeField] private FirstTutorialSettings[] firstTutorials;

        [BoxGroup("Tutorial")] [SerializeField] private TutorialSettings[] _uiElementsTutorial;
        
        private bool _isBlockClick;
        private int _tutorialIndex;
        private IDisposable _clickDisposable;

        internal bool IsFirstTutorial => isFirstTutorial && firstTutorials.Length > 0;
        internal int FirstTutorialCount => firstTutorials.Length;
        internal bool IsContainsTutorials => _uiElementsTutorial.Length > 0;

        internal void ShowFirstTutorial(int index)
        {
            if (isFirstTutorial is false) return;
            
            ClearTutorial();

            _isBlockClick = true;
            firstTutorials[index].button.gameObject.SetActive(true);
            _textTutorial.text = firstTutorials[index].text;

            _clickDisposable = firstTutorials[index].button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _isBlockClick = false;
                    
                    UIMessageBroker.Instance.Publish(new TutorialWindowControlMessage
                    {
                        Type = WindowType.None
                    });
                    
                    _clickDisposable.Dispose();
                });
        }

        internal void ShowTutorial()
        {
            if (_uiElementsTutorial is null || _uiElementsTutorial.Length == 0) return;
           
            ClearTutorial();
            
            if (_tutorialIndex >= _uiElementsTutorial.Length)
            {
                UIMessageBroker.Instance.Publish(new TutorialWindowControlMessage {Type = WindowType.None});
                return;
            }

            _uiElementsTutorial[_tutorialIndex].image.enabled = true;
            _textTutorial.text = _uiElementsTutorial[_tutorialIndex].text;
        }

        private void ClearTutorial()
        {
            for (int i = 0; i < firstTutorials?.Length; i++) 
                firstTutorials[i].button.gameObject.SetActive(false);

            _textTutorial.text = "";
            for (int i = 0; i < _uiElementsTutorial?.Length; i++)
                _uiElementsTutorial[i].image.enabled = false;
        }

        protected override void DisposeContainers()
        {
            base.DisposeContainers();
            
            ClearTutorial();
            _tutorialIndex = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isBlockClick is true) return;
            
            ++_tutorialIndex;
            ShowTutorial();
        }
        
        [Serializable]
        private struct FirstTutorialSettings
        {
            [SerializeField] [HideLabel, HorizontalGroup] public Button button;
            [SerializeField] [HorizontalGroup, HideLabel] public string text;
        }
        
        [Serializable]
        private struct TutorialSettings
        {
            [SerializeField] [PreviewField, HideLabel, HorizontalGroup] public Image image;
            [SerializeField] [HorizontalGroup(width:250), HideLabel, TextArea] public string text;
        }
    }
}