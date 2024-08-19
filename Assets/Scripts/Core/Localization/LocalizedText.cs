using System.Collections;
using Game;
using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker.Messages;
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
        [SerializeField] private string key;

        private Text _value;
        private readonly CompositeDisposable _disposable = new();

        private void Awake()
        {
            _value = GetComponent<Text>();
        }

        private IEnumerator Start()
        {
            MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(e => OnLangChanged())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<SceneLoadedMessage>()
                .Subscribe(e => OnLangChanged())
                .AddTo(_disposable);
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

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}