using MessageBroker;
using MessageBroker.Messages.UI;
using UI.Enums;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Room
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Image  background;
        [SerializeField] private Sprite mainImage;
        [SerializeField] private Sprite darkImage;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<WindowControlMessage>()
                .Subscribe(HandleWindow)
                .AddTo(_disposable);
        }

        private void HandleWindow(WindowControlMessage m)
        {
            background.sprite = m.Type switch
            {
                WindowType.CharacterCreator => darkImage,
                WindowType.Personal         => darkImage,
                _                           => mainImage
            };
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}