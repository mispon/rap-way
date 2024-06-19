using System;
using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public class UIManager : MonoBehaviour, IDisposable
    {
        [SerializeField] private UIElementContainer[] containers;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(msg => InitContainers())
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<SceneLoadedMessage>()
                .Subscribe(msg => InitContainers())
                .AddTo(_disposable);
            SceneMessageBroker.Instance
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposable);
        }

        private void InitContainers()
        {
            foreach (var container in containers)
            {
                container.Initialize();
            }
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            
            foreach (var container in containers)
                container.Dispose();
        }
    }
}