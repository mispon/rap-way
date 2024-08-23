using System.Collections;
using Game;
using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker.Messages;
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

        private TextMeshProUGUI _value;
        private readonly CompositeDisposable _disposable = new();

        private void Awake()
        {
            _value = GetComponent<TextMeshProUGUI>();
        }

        private IEnumerator Start()
        {
            MsgBroker.Instance
                .Receive<LangChangedMessage>()
                .Subscribe(e => OnLangChanged())
                .AddTo(_disposable);

            while (!GameManager.Instance.Ready)
                yield return null;

            // apply actual localization 
            OnLangChanged();
        }

        private void OnLangChanged()
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

            _value.text = text;
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}