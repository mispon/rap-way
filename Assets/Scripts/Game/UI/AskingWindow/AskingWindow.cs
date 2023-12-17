using System;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.AskingWindow
{
    public class AskingWindow : MonoBehaviour
    {
        [SerializeField] private Text questionText;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button acceptButton;

        private Action _action;

        private void Start()
        {
            rejectButton.onClick.AddListener(OnReject);
            acceptButton.onClick.AddListener(OnAccept);
        }

        public void Show(string question, Action action)
        {
            questionText.text = question;
            _action = action;
            
            gameObject.SetActive(true);
        }

        private void OnReject()
        {
            SoundManager.Instance.PlayClick();
            gameObject.SetActive(false);
        }

        private void OnAccept()
        {
            SoundManager.Instance.PlayClick();
            
            _action.Invoke();
            gameObject.SetActive(false);
        }
    }
}