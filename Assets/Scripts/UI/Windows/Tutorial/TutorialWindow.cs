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
    public class TutorialWindow : CanvasUIElement, IPointerClickHandler
    {
        [SerializeField] private Text _textTutorial;

        [BoxGroup("First tutorial")] [SerializeField] private bool _isFirstTutorial;
        [ShowIf("_isFirstTutorial"), BoxGroup("First tutorial")] [SerializeField] private Button _buttonFirstTutorial;
        [ShowIf("_isFirstTutorial"), BoxGroup("First tutorial"), TextArea] [SerializeField] private string _textFirstTutorial;

        [BoxGroup("Tutorial")] [SerializeField] private TutorialSettings[] _uiElementsTutorial;
        
        private bool _isBlockClick;
        private int _tutorialIndex;

        public bool IsFirstTutorial => _isFirstTutorial;
        public bool IsContainsTutorials => _uiElementsTutorial.Length > 0;

        public void ShowFirstTutorial()
        {
            if (_isFirstTutorial is false) return;
            
            ClearTutorial();

            _isBlockClick = true;
            _buttonFirstTutorial.gameObject.SetActive(true);
            _textTutorial.text = _textFirstTutorial;

            _buttonFirstTutorial.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _isBlockClick = false;
                    
                    UIMessageBroker.Instance.Publish(new TutorialWindowControlMessage
                    {
                        Type = WindowType.None
                    });
                });
        }

        public void ShowTutorial()
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
            _buttonFirstTutorial.gameObject.SetActive(false);

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
        
        private struct TutorialSettings
        {
            public Image image;
            public string text;
        }
    }
}