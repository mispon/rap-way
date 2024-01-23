using Game.UI.Enums;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.TutorialWindows
{
    public class TutorialWindow : CanvasUIElement, IPointerClickHandler
    {
        [SerializeField] private Text _textTutorial;
        [SerializeField] private TutorialSettings[] _uiElementsTutorial;
        
        [BoxGroup("First tutorial")] [SerializeField] private bool _isFirstTutorial;
        [ShowIf("_isFirstTutorial"), BoxGroup("First tutorial")] [SerializeField] private Button _buttonFirstTutorial;
        [ShowIf("_isFirstTutorial"), BoxGroup("First tutorial"), TextArea] [SerializeField] private string _textFirstTutorial;

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
                    _buttonFirstTutorial.gameObject.SetActive(false);

                    _isBlockClick = false;
                    
                    uiMessageBus.Publish(new TutorialWindowControlMessage()
                    {
                        Type = TutorialWindowType.None
                    });
                });
        }

        public void ShowTutorial()
        {
            if (_uiElementsTutorial is null || _uiElementsTutorial.Length == 0) return;
           
            ClearTutorial();
            
            if (_tutorialIndex >= _uiElementsTutorial.Length)
            {
                UIMessageBroker.Instance.MessageBroker
                    .Publish(new TutorialWindowControlMessage()
                    {
                        Type = TutorialWindowType.None
                    });
                
                return;
            }

            _uiElementsTutorial[_tutorialIndex].image.enabled = true;
            _textTutorial.text = _uiElementsTutorial[_tutorialIndex].text;
        }

        private void ClearTutorial()
        {
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