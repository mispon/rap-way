using System;
using MessageBroker;
using MessageBroker.Messages.Game;
using Scenes.MessageBroker;
using Scenes.MessageBroker.Messages;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace UI.Base
{
    public class UIManager : SerializedMonoBehaviour, IDisposable
    {
        [SerializeField] private UIElementContainer[] containers;

        private readonly CompositeDisposable _disposable = new();

        private void Start()
        {
            MsgBroker.Instance
                .Receive<GameReadyMessage>()
                .Subscribe(_ =>
                {
                    foreach (var container in containers)
                        container.Initialize();
                })
                .AddTo(_disposable);
            
            SceneMessageBroker.Instance
                .Receive<SceneLoadMessage>()
                .Subscribe(msg => Dispose())
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            
            foreach (var container in containers)
                container.Dispose();
        }
    }
}