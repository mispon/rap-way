using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker.Messages;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Controls.UnreadBadge
{
    public abstract class BaseUnreadBadge : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI value;

        protected readonly CompositeDisposable disposables = new();

        private int _counter;

        private void Start()
        {
            MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(e => OnGameReady())
                .AddTo(disposables);
            MsgBroker.Instance
                .Receive<SceneLoadedMessage>()
                .Subscribe(e => OnGameReady())
                .AddTo(disposables);
        }

        private void OnGameReady()
        {
            Hide();
            RegisterHandlers();
            Init();
        }

        protected abstract void Init();
        protected abstract void RegisterHandlers();

        protected void IncCounter()
        {
            value.text = $"{++_counter}";
            Show();
        }

        protected void DecCounter()
        {
            value.text = $"{--_counter}";

            if (_counter == 0)
            {
                Hide();
            }
        }

        protected void UpdateCounter(int val)
        {
            _counter = val;
            value.text = $"{_counter}";

            if (_counter > 0)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Show()
        {
            image.enabled = true;
            value.enabled = true;
        }

        private void Hide()
        {
            image.enabled = false;
            value.enabled = false;
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}