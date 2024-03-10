using System;
using MessageBroker;
using MessageBroker.Messages.Game;
using UniRx;
using UnityEngine;

namespace Core.OrderedStarter
{
    public class OrderedStarter : MonoBehaviour
    {
        [Header("Init order")]
        [SerializeField] private MonoBehaviour[] starters;

        private IDisposable _disposable;
        
        private void Start()
        {
            _disposable = MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(e =>
                {
                    foreach (var starter in starters)
                    {
                        (starter as IStarter)?.OnStart();
                    }
                });
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}