using System.Collections.Generic;
using Game.UI.Enums;
using Game.UI.Messages;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI.TutorialWindows
{
    public abstract class TutorialWindow : CanvasUIElement, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _textTutorial;
        [SerializeField] private KeyValuePair<Image, string>[] _uiElementsTutorial;

        [BoxGroup("First tutorial")] private const string SAVE_KEY_FIRST_TUTORIAL = "FirstTutorial";
        [BoxGroup("First tutorial")] private string _textFirstTutorial;

        private bool _isFirstTutorial;
        private int _tutorialIndex;

        public override void Show()
        {
            base.Show();
           
            _isFirstTutorial = PlayerPrefs.HasKey($"{SAVE_KEY_FIRST_TUTORIAL}{this.name}") is false;
           
            if (_isFirstTutorial) ShowFirstTutorial();
            else ShowTutorial(_tutorialIndex);
        }

        private void ShowFirstTutorial()
        {
            PlayerPrefs.SetInt($"{SAVE_KEY_FIRST_TUTORIAL}{this.name}", 1);
            _textTutorial.text = _textFirstTutorial;
        }

        private void ShowTutorial(int index)
        {
            if (_uiElementsTutorial is null || _uiElementsTutorial.Length == 0) return;
            
            HideLastTutorial();
            
            if (index > _uiElementsTutorial.Length) 
                UIMessageBroker.Instance.MessageBroker
                    .Publish(new TutorialWindowControlMessage()
                    {
                        Type = TutorialWindowType.None
                    });

            _uiElementsTutorial[index].Key.enabled = true;
            _textTutorial.text = _uiElementsTutorial[index].Value;
        }

        private void HideLastTutorial()
        {
            _textTutorial.text = "";
            for (int i = 0; i < _uiElementsTutorial?.Length; i++)
                _uiElementsTutorial[i].Key.enabled = false;
        }

        protected override void DisposeContainers()
        {
            base.DisposeContainers();
            
            HideLastTutorial();
            _tutorialIndex = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isFirstTutorial is true) return;
            
            ShowTutorial(++_tutorialIndex);
        }
    }
}