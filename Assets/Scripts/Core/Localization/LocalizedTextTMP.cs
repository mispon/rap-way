using System;
using System.Collections;
using MessageBroker;
using MessageBroker.Messages.Game;
using TMPro;
using UniRx;
using UnityEngine;

namespace Core.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTextTMP : MonoBehaviour
    {
        [SerializeField] private TextCase textCase = TextCase.Normal;
        [SerializeField] private string key;
        
        private TextMeshProUGUI value;
        private IDisposable _disposable;

        private void Awake()
        {
            value = GetComponent<TextMeshProUGUI>();

            _disposable = MsgBroker.Instance
                .Receive<LangChangedMessage>()
                .Subscribe(e => OnLangChanged());
        }

        private IEnumerator Start()
        {
            while (!LocalizationManager.Instance.IsReady)
                yield return null;
            
            ApplyText();
        }
        
        private void OnLangChanged()
        {
            ApplyText();
        }
        
        private void ApplyText()
        {
            string text = LocalizationManager.Instance.Get(key);
            
            switch (textCase)
            {
                case TextCase.Lower:
                    text = text.ToLowerInvariant();
                    break;
                case TextCase.Upper:
                    text = text.ToUpperInvariant();
                    break;
            }

            value.text = text;
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}