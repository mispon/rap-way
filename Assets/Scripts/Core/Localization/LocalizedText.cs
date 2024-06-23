using System;
using System.Collections;
using MessageBroker;
using MessageBroker.Messages.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Localization
{
    public enum TextCase
    {
        Normal,
        Lower,
        Upper
    }

    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private TextCase textCase = TextCase.Normal;
        [SerializeField] private string   key;

        private IDisposable _disposable;
        private Text        _value;

        private IEnumerator Start()
        {
            while (!LocalizationManager.Instance.IsReady)
            {
                yield return null;
            }

            _value = GetComponent<Text>();

            _disposable = MsgBroker.Instance
                .Receive<LangChangedMessage>()
                .Subscribe(e => OnLangChanged());

            ApplyText();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void OnLangChanged()
        {
            ApplyText();
        }

        private void ApplyText()
        {
            var text = LocalizationManager.Instance.Get(key);

            switch (textCase)
            {
                case TextCase.Lower:
                    text = text.ToLowerInvariant();
                    break;
                case TextCase.Upper:
                    text = text.ToUpperInvariant();
                    break;
            }

            _value.text = text;
        }
    }
}