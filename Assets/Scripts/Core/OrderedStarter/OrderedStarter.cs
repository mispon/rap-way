using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace Core.OrderedStarter
{
    public class OrderedStarter : MonoBehaviour
    {
        [Header("Init order")]
        [SerializeField] private MonoBehaviour[] starters;

        private readonly CompositeDisposable _disposable = new();
        
        private void Start()
        {
            MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(_ => Run())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<SceneLoadedMessage>()
                .Subscribe(_ => Run())
                .AddTo(_disposable);
        }

        private void Run()
        {
            foreach (var starter in starters)
            {
                (starter as IStarter)?.OnStart();
            }
        }

        private void OnDestroy()
        {
            _disposable.Clear();
        }
    }
}